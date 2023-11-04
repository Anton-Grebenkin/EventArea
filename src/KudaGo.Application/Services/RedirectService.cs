using KudaGo.Application.Abstractions;
using KudaGo.Application.Messages;

namespace KudaGo.Application.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly IRegisterService<string, IMessageHandler> _commandRegisterService;
        private readonly IServiceProvider _serviceProvider;

        public RedirectService
        (
            IRegisterService<string, IMessageHandler> commandRegisterService,
            IServiceProvider serviceProvider
        )
        {
            _commandRegisterService = commandRegisterService;
            _serviceProvider = serviceProvider;
        }

        public async Task RedirectAsync(string commandName, MessageContext messageContext, CancellationToken cancellationToken)
        {
            var hadlerType = _commandRegisterService.Tpes
                .Where(t => t.Key == commandName)
                .FirstOrDefault()
                .Value;

            if ( hadlerType != null )
            {
                var handler = (IMessageHandler)_serviceProvider.GetService(hadlerType);

                await handler.HandleAsync(messageContext, cancellationToken);
            }
        }
    }
}
