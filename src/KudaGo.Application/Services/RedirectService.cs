using KudaGo.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Telegram.Bot.Types;

namespace KudaGo.Application.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly IServiceProvider _serviceProvider;
        public RedirectService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task RedirectAsync(string commandName, Message message, CancellationToken cancellationToken)
        {
            var type = _serviceProvider.GetService<IRegisterService<ICommandHandler, string>>()
                .Tpes
                .Where(p => p.Key == commandName)
                .FirstOrDefault()
                .Value;

            var handler = (ICommandHandler)_serviceProvider.GetService(type);

            //await handler.HandleAsync(message, cancellationToken);
        }
    }
}
