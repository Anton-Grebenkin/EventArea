using Telegram.Bot.Types.ReplyMarkups;

namespace KudaGo.Application.Messages
{
    public record MessageData(string Text, IReplyMarkup ReplyMarkup = null);
}
