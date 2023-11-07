using KudaGo.Application.Common.Abstractions;
using KudaGo.Application.Common.Messages;

namespace KudaGo.Application.Services
{
    public class CallbackHandlersRegisterService : IRegisterService<CallbackType, IMessageHandler>
    {
        public IDictionary<CallbackType, Type> Tpes => _tpes;
        private readonly IDictionary<CallbackType, Type> _tpes;

        public CallbackHandlersRegisterService(IDictionary<CallbackType, Type> tpes)
        {
            _tpes = tpes;
        }
    }
}
