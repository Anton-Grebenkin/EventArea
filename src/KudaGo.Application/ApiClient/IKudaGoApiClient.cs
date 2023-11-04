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
        Task<IEnumerable<EventCategory>> GetEventCategoriesAsync();

        [Get("/events")]
        Task<GetEventsResult> GetEventsAsync(
            [AliasAs("actual_since")] long since,
            string location,
            [AliasAs("order_by")] string orderBy = "-id",
            string categories = "",
            string fields = "id,publication_date,dates,title,place,description,images,site_url,categories",
            int page = 1,
            [AliasAs("page_size")] int pageSize = 100,
            [AliasAs("text_format")] string textFormat = "text"
            );
    }
}
