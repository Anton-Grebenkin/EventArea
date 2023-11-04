using KudaGo.Application.Abstractions;
using KudaGo.Application.ApiClient;
using KudaGo.Application.Attributes;
using KudaGo.Application.Data;
using KudaGo.Application.Messages;
using KudaGo.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Refit;
using System.Reflection;

namespace KudaGo.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services
            .AddRefitClient<IKudaGoApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://kudago.com/public-api/v1.4"));

            services.AddSingleton(new MongoClient("mongodb://localhost:27017").GetDatabase("kudago"));
            services.AddScoped<UserRepository, UserRepository>();

            return services;
        }

        public static IServiceCollection RegisterAssemblyTypes<T>(this IServiceCollection services, ServiceLifetime lifetime, List<Func<TypeInfo, bool>> predicates = null)
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
                    .Where(x => typeof(T).IsAssignableFrom(x))
                );

            if (predicates?.Count() > 0)
            {
                foreach (var predict in predicates)
                {
                    types = types.Where(predict);
                }
            }

            foreach (var type in types)
            {
                services.TryAdd(new ServiceDescriptor(
                    typeof(T),
                    type,
                    lifetime)
                );
            }
            

            return services;
        }

        public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
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

        public static IServiceCollection AddCallbackHandlers(this IServiceCollection services)
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
