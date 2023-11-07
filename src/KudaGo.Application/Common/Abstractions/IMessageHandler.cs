using KudaGo.Application.Common.Messages;

namespace KudaGo.Application.Common.Abstractions
{
    public interface IMessageHandler
    {
        Task HandleAsync(MessageContext updateContext, CancellationToken cancellationToken);
    }
}
