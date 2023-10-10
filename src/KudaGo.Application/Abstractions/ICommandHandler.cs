
namespace KudaGo.Application.Abstractions
{
    public interface ICommandHandler
    {
        Task HandleAsync(MessageContext messageContext, CancellationToken cancellationToken);
    }
}
