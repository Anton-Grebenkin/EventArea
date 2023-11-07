using KudaGo.Application.Common.ApiClient.Dto;
using Refit;

namespace KudaGo.Application.Common.ApiClient
{
    public interface IKudaGoApiClient
    {
        [Get("/locations")]
        Task<IEnumerable<City>> GetCitiesAsync(CancellationToken cancellationToken = default);

        [Get("/locations/{slug}")]
        Task<City> GetCityAsync(string slug, CancellationToken cancellationToken = default);

        [Get("/event-categories")]
        Task<IEnumerable<EventCategory>> GetEventCategoriesAsync(CancellationToken cancellationToken = default);

        [Get("/events")]
        Task<GetEventsResult> GetEventsAsync(
            [AliasAs("actual_since")] long since,
            string location,
            [AliasAs("order_by")] string orderBy = "-id",
            string categories = "",
            string fields = "id,publication_date,dates,title,place,description,images,site_url,categories",
            int page = 1,
            [AliasAs("page_size")] int pageSize = 100,
            [AliasAs("text_format")] string textFormat = "text",
            CancellationToken cancellationToken = default
            );
    }
}
