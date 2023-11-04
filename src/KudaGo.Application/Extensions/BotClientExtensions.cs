using KudaGo.Application.Messages;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace KudaGo.Application.Extensions
{
    public static class BotClientExtensions
    {
        public static async Task SendTypingActionAsync(this ITelegramBotClient telegramBotClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await telegramBotClient.SendChatActionAsync(
                  chatId: chatId,
                  chatAction: ChatAction.Typing,
                  cancellationToken: cancellationToken);
        }

        public static async Task SendMessageAsync(this ITelegramBotClient telegramBotClient, ChatId chatId, MessageData messageData, CancellationToken cancellationToken)
        {
            await telegramBotClient.SendTextMessageAsync(
               chatId: chatId,
               text: messageData.Text,
               replyMarkup: messageData.ReplyMarkup,
               cancellationToken: cancellationToken);
        }

        public static async Task EditMessageAsync(this ITelegramBotClient telegramBotClient, ChatId chatId, int messageId, MessageData messageData, CancellationToken cancellationToken)
        {  
            await telegramBotClient.EditMessageTextAsync(
                chatId,
                messageId,
                messageData.Text,
                replyMarkup: (InlineKeyboardMarkup?)messageData.ReplyMarkup,
                cancellationToken: cancellationToken);
        }


    }
}
