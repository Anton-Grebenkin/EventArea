using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot;
using KudaGo.Application.Common.Abstractions;
using KudaGo.Application.Common.Messages;

namespace KudaGo.TelegramBot.Services
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly ILogger<UpdateHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        public UpdateHandler(ILogger<UpdateHandler> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
        {
            var handler = update switch
            {
                { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
                { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update, cancellationToken)
            };

            Task.Factory.StartNew(async () => await handler, cancellationToken);
        }

        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Receive message type: {MessageType}", message.Type);

            if (string.IsNullOrEmpty(message.Text))
                return;

            using var scope = _serviceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var type = serviceProvider.GetRequiredService<IRegisterService<string, IMessageHandler>>()
                .Tpes
                .Where(p => p.Key == message.Text.Split(' ')[0])
                .FirstOrDefault()
                .Value;

            if (type == null) 
            {
                var botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                return;
            }

            var handler = (IMessageHandler)serviceProvider.GetRequiredService(type);

            var context = new MessageContext
            {
                MessageId = message.MessageId,
                ChatId = message.Chat.Id,
                User = message.From
            };

            await handler.HandleAsync(context, cancellationToken);
        }

        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);

            using var scope = _serviceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();

            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                cancellationToken: cancellationToken);

            var callbackData = CallbackData.FromJsonString(callbackQuery.Data);

            var type = serviceProvider.GetRequiredService<IRegisterService<CallbackType, IMessageHandler>>()
                .Tpes
                .Where(p => p.Key == callbackData.CallbackType)
                .FirstOrDefault()
                .Value;

            if (type == null) return;

            var handler = (IMessageHandler)serviceProvider.GetRequiredService(type);

            var context = new MessageContext
            {
                MessageId = callbackQuery.Message.MessageId,
                ChatId = callbackQuery.Message.Chat.Id,
                User = callbackQuery.From,
                CallbackData = callbackData
            };

            await handler.HandleAsync(context, cancellationToken);

        }

        private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
            return Task.CompletedTask;
        }

        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

            // Cooldown in case of network connection error
            if (exception is RequestException)
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }
}
