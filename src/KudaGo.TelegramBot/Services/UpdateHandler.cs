using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types;
using Telegram.Bot;
using KudaGo.Application.Abstractions;
using KudaGo.Application.Messages;
using KudaGo.Application;

namespace KudaGo.TelegramBot.Services
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<UpdateHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger, IServiceProvider serviceProvider)
        {
            _botClient = botClient;
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

            await handler;
        }

        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Receive message type: {MessageType}", message.Type);
            if (message.Text is not { } messageText)
                return;

            var type = _serviceProvider.GetService<IRegisterService<string, IMessageHandler>>()
                .Tpes
                .Where(p => p.Key == messageText.Split(' ')[0])
                .FirstOrDefault()
                .Value;

            var handler = (IMessageHandler)_serviceProvider.GetService(type);

            var context = new MessageContext
            {
                MessageId = message.MessageId,
                ChatId = message.Chat.Id,
                User = message.From
            };

            await handler.HandleAsync(context, cancellationToken);

            //static async Task<Message> SendReplyKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            //{
            //    ReplyKeyboardMarkup replyKeyboardMarkup = new(
            //        new[]
            //        {
            //            new KeyboardButton[] { "1.1", "1.2" },
            //            new KeyboardButton[] { "2.1", "2.2" },
            //        })
            //    {
            //        ResizeKeyboard = true
            //    };

            //    return await botClient.SendTextMessageAsync(
            //        chatId: message.Chat.Id,
            //        text: "Choose",
            //        replyMarkup: replyKeyboardMarkup,
            //        cancellationToken: cancellationToken);
            //}

            //static async Task<Message> RemoveKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            //{
            //    return await botClient.SendTextMessageAsync(
            //        chatId: message.Chat.Id,
            //        text: "Removing keyboard",
            //        replyMarkup: new ReplyKeyboardRemove(),
            //        cancellationToken: cancellationToken);
            //}
        }

        // Process Inline Keyboard callback data
        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);

            await _botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                cancellationToken: cancellationToken);

            var callbackData = CallbackData.FromJsonString(callbackQuery.Data);

            var type = _serviceProvider.GetService<IRegisterService<CallbackType, IMessageHandler>>()
                .Tpes
                .Where(p => p.Key == callbackData.CallbackType)
                .FirstOrDefault()
                .Value;

            var handler = (IMessageHandler)_serviceProvider.GetService(type);

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
