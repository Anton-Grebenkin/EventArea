
using KudaGo.Application.Messages;

namespace KudaGo.Application.Abstractions
{
    public interface IMessageHandler
    {
        Task HandleAsync(MessageContext updateContext, CancellationToken cancellationToken);
    }
}
