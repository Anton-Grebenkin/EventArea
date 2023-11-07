using KudaGo.Application.Common.Configuration;
using KudaGo.Application.Common.Extensions;
using KudaGo.TelegramBot.Services;
using KudaGo.TelegramBot.Workers;
using Telegram.Bot;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;

        services.Configure<BotConfiguration>(
            config.GetSection(nameof(BotConfiguration)));

        services.Configure<MongoConfiguration>(
            config.GetSection(nameof(MongoConfiguration)));

        services.Configure<KudaGoConfiguration>(
           config.GetSection(nameof(KudaGoConfiguration)));

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

        services.AddMemoryCache();

        services.AddHostedService<UpdateEventsWorker>();
        services.AddHostedService<EventRecomendationWorker>();

        services.AddApplication();
    })
    .Build();

await host.RunAsync();

