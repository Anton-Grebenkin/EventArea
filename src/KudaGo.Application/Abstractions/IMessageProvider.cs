using KudaGo.Application.ApiClient.Dto;
using KudaGo.Application.Messages;
using Telegram.Bot.Types;

namespace KudaGo.Application.Abstractions
{
    public interface IMessageProvider
    {
        Task<MessageData> WelcomeMessageAsync();
        Task<MessageData> CitySelectionMessageAsync(IEnumerable<City> cities, CallbackType callbackType);
        Task<MessageData> CitySelectedMessageAsync(string cyty, CallbackType callbackType);
        Task<MessageData> SelectCategoriesMessageAsync(IEnumerable<ItemSelection<EventCategory>> categories, int currentPage, bool nextPage, bool previosPage, CallbackType citySelection);
        Task<MessageData> CompleteSelectCategoriesMessageAsync();
        Task<IEnumerable<InputMediaPhoto>> EventReccomendationMessageAsync(Data.Entites.Event @event);
    }
}
