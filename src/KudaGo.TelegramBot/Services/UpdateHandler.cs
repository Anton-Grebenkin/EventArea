using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using KudaGo.Application.Data;
using KudaGo.Application.ApiClient;
using KudaGo.Application.Abstractions;
using System.Reflection;
using KudaGo.Application.Attributes;
using KudaGo.Application.Messages;
using Microsoft.Extensions.DependencyInjection;
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
                { InlineQuery: { } inlineQuery } => BotOnInlineQueryReceived(inlineQuery, cancellationToken),
                { ChosenInlineResult: { } chosenInlineResult } => BotOnChosenInlineResultReceived(chosenInlineResult, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update, cancellationToken)
            };

            await handler;
        }

        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, "ghsfsd");

            _logger.LogInformation("Receive message type: {MessageType}", message.Type);
            if (message.Text is not { } messageText)
                return;

            var type = _serviceProvider.GetService<IRegisterService<ICommandHandler, string>>()
                .Tpes
                .Where(p => p.Key == messageText.Split(' ')[0])
                .FirstOrDefault()
                .Value;

            var handler = (ICommandHandler)_serviceProvider.GetService(type);

            var context = new MessageContext()
            {
                Update = message
            };


            await handler.HandleAsync(context, cancellationToken);

            //_logger.LogInformation("The message was sent with id: {SentMessageId}", "");

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

            //static async Task<Message> SendFile(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            //{
            //    await botClient.SendChatActionAsync(
            //        message.Chat.Id,
            //        ChatAction.UploadPhoto,
            //        cancellationToken: cancellationToken);

            //    const string filePath = "Files/tux.png";
            //    await using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            //    var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

            //    return await botClient.SendPhotoAsync(
            //        chatId: message.Chat.Id,
            //        photo: new InputFileStream(fileStream, fileName),
            //        caption: "Nice Picture",
            //        cancellationToken: cancellationToken);
            //}

            //static async Task<Message> RequestContactAndLocation(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            //{
            //    ReplyKeyboardMarkup RequestReplyKeyboard = new(
            //        new[]
            //        {
            //        KeyboardButton.WithRequestLocation("Location"),
            //        KeyboardButton.WithRequestContact("Contact"),
            //        });

            //    return await botClient.SendTextMessageAsync(
            //        chatId: message.Chat.Id,
            //        text: "Who or Where are you?",
            //        replyMarkup: RequestReplyKeyboard,
            //        cancellationToken: cancellationToken);
            //}

            //static async Task<Message> Usage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            //{
            //    const string usage = "Usage:\n" +
            //                         "/inline_keyboard - send inline keyboard\n" +
            //                         "/keyboard    - send custom keyboard\n" +
            //                         "/remove      - remove custom keyboard\n" +
            //                         "/photo       - send a photo\n" +
            //                         "/request     - request location or contact\n" +
            //                         "/inline_mode - send keyboard with Inline Query";

            //    return await botClient.SendTextMessageAsync(
            //        chatId: message.Chat.Id,
            //        text: usage,
            //        replyMarkup: new ReplyKeyboardRemove(),
            //        cancellationToken: cancellationToken);
            //}

            //static async Task<Message> StartInlineQuery(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            //{
            //    InlineKeyboardMarkup inlineKeyboard = new(
            //        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode"));

            //    return await botClient.SendTextMessageAsync(
            //        chatId: message.Chat.Id,
            //        text: "Press the button to start Inline Query",
            //        replyMarkup: inlineKeyboard,
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

            var type = _serviceProvider.GetService<IRegisterService<ICallbackHandler, CallbackType>>()
                .Tpes
                .Where(p => p.Key == callbackData.CallbackType)
                .FirstOrDefault()
                .Value;

            var handler = (ICallbackHandler)_serviceProvider.GetService(type);
               
            //await handler.HandleAsync(callbackQuery, callbackData, cancellationToken);

        }


        #region Inline Mode

        private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

            InlineQueryResult[] results = {
            // displayed result
            new InlineQueryResultArticle(
                id: "1",
                title: "TgBots",
                inputMessageContent: new InputTextMessageContent("hello"))
        };

            await _botClient.AnswerInlineQueryAsync(
                inlineQueryId: inlineQuery.Id,
                results: results,
                cacheTime: 0,
                isPersonal: true,
                cancellationToken: cancellationToken);
        }

        private async Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);

            await _botClient.SendTextMessageAsync(
                chatId: chosenInlineResult.From.Id,
                text: $"You chose result with Id: {chosenInlineResult.ResultId}",
                cancellationToken: cancellationToken);
        }

        #endregion

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
