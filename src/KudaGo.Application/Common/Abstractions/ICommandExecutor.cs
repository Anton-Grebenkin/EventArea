using KudaGo.Application.Common.Messages;

namespace KudaGo.Application.Common.Abstractions
{
    public interface ICommandExecutor
    {
        Task ExecuteNextCommandAsync(string commandName, MessageContext messageContext, CancellationToken cancellationToken);
    }
}
