
using KudaGo.Application.Messages;

namespace KudaGo.Application.Abstractions
{
    public interface IRedirectService
    {
        Task RedirectAsync(string commandName, MessageContext messageContext, CancellationToken cancellationToken);
    }
}
