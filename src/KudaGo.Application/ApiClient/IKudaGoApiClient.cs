using KudaGo.Application.ApiClient.Dto;
using Refit;

namespace KudaGo.Application.ApiClient
{
    public interface IKudaGoApiClient
    {
        [Get("/locations")]
        Task<IEnumerable<City>> GetCitiesAsync();

        [Get("/locations/{slug}")]
        Task<City> GetCityAsync(string slug);

        [Get("/event-categories")]
        Task<IEnumerable<EventCategory>> GetEventCategoriesAsync(string slug);
    }
}
