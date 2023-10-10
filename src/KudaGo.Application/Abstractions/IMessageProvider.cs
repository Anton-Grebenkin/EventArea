using KudaGo.Application.ApiClient.Dto;
using KudaGo.Application.Messages;

namespace KudaGo.Application.Abstractions
{
    public interface IMessageProvider
    {
        Task<MessageData> WelcomeMessageAsync();
        Task<MessageData> SitySelectionMessageAsync(IEnumerable<City> cities, string nextCommand);
    }
}
