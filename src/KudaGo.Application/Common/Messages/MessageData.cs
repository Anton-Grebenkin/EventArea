using Telegram.Bot.Types.ReplyMarkups;

namespace KudaGo.Application.Common.Messages
{
    public record MessageData(string Text, IReplyMarkup ReplyMarkup = null);
}
