using Telegram.Bot.Types;

namespace KudaGo.Application.Messages
{
    public class MessageContext
    {
        public long ChatId { get; set; }
        public int MessageId { get; set; }
        public User User { get; set; }
        public CallbackData CallbackData { get; set; }
    }

}
