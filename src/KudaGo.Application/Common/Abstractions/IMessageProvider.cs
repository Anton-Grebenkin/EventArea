
using KudaGo.Application.Common.ApiClient.Dto;
using KudaGo.Application.Common.Messages;
using Telegram.Bot.Types;

namespace KudaGo.Application.Common.Abstractions
{
    public interface IMessageProvider
    {
        Task<MessageData> WelcomeMessageAsync(CancellationToken cancellationToken = default);
        Task<MessageData> CitySelectionMessageAsync(IEnumerable<City> cities, CallbackType callbackType, CancellationToken cancellationToken = default);
        Task<MessageData> CitySelectedMessageAsync(string cyty, CallbackType callbackType, CancellationToken cancellationToken = default);
        Task<MessageData> SelectCategoriesMessageAsync(
            IEnumerable<ItemSelection<EventCategory>> categories, 
            int currentPage, 
            bool nextPage, 
            bool previosPage, 
            CallbackType citySelection,
            CancellationToken cancellationToken = default);
        Task<MessageData> CompleteSelectCategoriesMessageAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<InputMediaPhoto>> EventReccomendationMessageAsync(Common.Data.Entites.Event @event, CancellationToken cancellationToken = default);
    }
}
