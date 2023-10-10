
using KudaGo.Application.Messages;
using Telegram.Bot.Types;

namespace KudaGo.Application
{
    public class UpdateContext<TUpdate, TContextData> 
    {
        public TUpdate Update { get; set; }
        public TContextData ContextData { get; set; }
    }

    public class CallbackContext : UpdateContext<CallbackQuery, ContextData> 
    {
        public CallbackQuery Update { get; set; }
        public ContextData ContextData { get; set; }
        public CallbackData CallbackData { get; set; }
    }

    public class MessageContext : UpdateContext<Message, ContextData>
    {
        public Message Update { get; set; }
        public ContextData ContextData { get; set; }
    }


}
