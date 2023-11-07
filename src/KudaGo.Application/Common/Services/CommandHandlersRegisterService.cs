using KudaGo.Application.Common.Abstractions;

namespace KudaGo.Application.Services
{
    public class CommandHandlersRegisterService : IRegisterService<string, IMessageHandler>
    {
        public IDictionary<string, Type> Tpes => _tpes;
        private readonly IDictionary<string, Type> _tpes;

        public CommandHandlersRegisterService(IDictionary<string, Type> tpes)
        {
            _tpes = tpes;
        }
    }
}
