using KudaGo.Application.Abstractions;
using KudaGo.Application.ApiClient;
using KudaGo.Application.Data;
using KudaGo.Application.Extensions;
using KudaGo.Application.Messages;
using KudaGo.Application.Services;
using KudaGo.TelegramBot;
using KudaGo.TelegramBot.Extensions;
using KudaGo.TelegramBot.Services;
using MongoDB.Driver;
using Refit;
using System;
using System.Reflection;
using Telegram.Bot;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<BotConfiguration>(
            context.Configuration.GetSection(BotConfiguration.Configuration));

        services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, provider) =>
                {
                    BotConfiguration? botConfig = provider.GetConfiguration<BotConfiguration>();
                    TelegramBotClientOptions options = new(botConfig.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });
        services.AddScoped<UpdateHandler>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();

        services
            .AddRefitClient<IKudaGoApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://kudago.com/public-api/v1.4"));

        services.AddSingleton(new MongoClient("mongodb://localhost:27017").GetDatabase("kudago"));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMessageTemplateRepository, MessageTemplateRepository>();
        services.AddScoped<IMessageProvider, MessageProvider>();
        services.AddScoped<IRedirectService, RedirectService>();

        services.AddCommandHandlers();
        services.AddCallbackHandlers();
    })
    .Build();

await host.RunAsync();

