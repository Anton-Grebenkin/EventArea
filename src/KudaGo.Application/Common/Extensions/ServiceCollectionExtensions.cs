using KudaGo.Application.Common.Abstractions;
using KudaGo.Application.Common.ApiClient;
using KudaGo.Application.Common.Attributes;
using KudaGo.Application.Common.Configuration;
using KudaGo.Application.Common.Data;
using KudaGo.Application.Common.Messages;
using KudaGo.Application.Features.EventsRecommendation;
using KudaGo.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Refit;
using System.Reflection;

namespace KudaGo.Application.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            var kudaGoConfiguration = provider.GetConfiguration<KudaGoConfiguration>();

            services
            .AddRefitClient<IKudaGoApiClient>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(kudaGoConfiguration.BaseUrl);
            });
            services.Decorate<IKudaGoApiClient, CachedKudaGoApiClient>();

            var mongoConfiguration = provider.GetConfiguration<MongoConfiguration>();
            services.AddSingleton(new MongoClient(mongoConfiguration.ConnectionString).GetDatabase(mongoConfiguration.DataBaseName));

            services.AddScoped<IUserRepository, UserRepository>();
            services.Decorate<IUserRepository, CachedUserRepository>();
            services.AddScoped<IMessageTemplateRepository, MessageTemplateRepository>();
            services.AddScoped<IEventRepository, EventRepository>();

            services.AddSingleton<IUpdateEventsService, UpdateEventsService>();
            services.AddSingleton<IEventRecommendationService, EventRecommendationService>();

            services.AddScoped<IMessageProvider, MessageProvider>();
            services.AddScoped<ICommandExecutor, CommandExecutor>();

            services.AddCommandHandlers();
            services.AddCallbackHandlers();

            return services;
        }

        private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            var scanAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            scanAssemblies.SelectMany(x => x.GetReferencedAssemblies())
                .Where(t => false == scanAssemblies.Any(a => a.FullName == t.FullName))
                .Distinct()
                .ToList()
                .ForEach(x => scanAssemblies.Add(AppDomain.CurrentDomain.Load(x)));

            var types = scanAssemblies
                .SelectMany(o => o.DefinedTypes
                    .Where(x => x.IsClass)
                    .Where(x => x.GetCustomAttribute<CommandTypeAttribute>() != null)
                    .Where(x => x.GetCustomAttribute<NotUsedAttribute>() == null)
                    .Where(x => typeof(IMessageHandler).IsAssignableFrom(x))
                    .Select(x => x.AsType())
                );

            var commandHandlerTypes = new Dictionary<string, Type>();

            foreach (var type in types)
            {
                commandHandlerTypes.Add(type.GetCustomAttribute<CommandTypeAttribute>().CommandType.GetCommandString(), type);

                services.TryAdd(new ServiceDescriptor(
                    type,
                    type,
                    ServiceLifetime.Scoped)
                );
            }

            var register = new CommandHandlersRegisterService(commandHandlerTypes);

            services.AddSingleton<IRegisterService<string, IMessageHandler>>(register);

            return services;
        }

        private static IServiceCollection AddCallbackHandlers(this IServiceCollection services)
        {
            var scanAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            scanAssemblies.SelectMany(x => x.GetReferencedAssemblies())
                .Where(t => false == scanAssemblies.Any(a => a.FullName == t.FullName))
                .Distinct()
                .ToList()
                .ForEach(x => scanAssemblies.Add(AppDomain.CurrentDomain.Load(x)));

            var types = scanAssemblies
                .SelectMany(o => o.DefinedTypes
                    .Where(x => x.IsClass)
                    .Where(x => x.GetCustomAttribute<CallbackTypeAttribute>() != null)
                    .Where(x => typeof(IMessageHandler).IsAssignableFrom(x))
                    .Select(x => x.AsType())
                );

            var callbackHandlerTypes = new Dictionary<CallbackType, Type>();

            foreach (var type in types)
            {
                callbackHandlerTypes.Add(type.GetCustomAttribute<CallbackTypeAttribute>().CallbackType, type);

                services.TryAdd(new ServiceDescriptor(
                    type,
                    type,
                    ServiceLifetime.Scoped)
                );
            }

            var register = new CallbackHandlersRegisterService(callbackHandlerTypes);

            services.AddSingleton<IRegisterService<CallbackType, IMessageHandler>>(register);

            return services;
        }

    }
}
