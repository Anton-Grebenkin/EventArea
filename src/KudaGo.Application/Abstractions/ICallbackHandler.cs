

namespace KudaGo.Application.Abstractions
{
    public interface ICallbackHandler
    {
        Task HandleAsync(CallbackContext callbackContext, CancellationToken cancellationToken);
    }
}
