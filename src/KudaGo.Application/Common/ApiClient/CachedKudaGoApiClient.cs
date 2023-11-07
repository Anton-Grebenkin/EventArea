
using KudaGo.Application.Common.ApiClient.Dto;
using Microsoft.Extensions.Caching.Memory;

namespace KudaGo.Application.Common.ApiClient
{
    public class CachedKudaGoApiClient : IKudaGoApiClient
    {
        private readonly IKudaGoApiClient _decorated;
        private readonly IMemoryCache _memoryCashe;

        public CachedKudaGoApiClient(IKudaGoApiClient kudaGoApiClient, IMemoryCache memoryCache)
        {
            _decorated = kudaGoApiClient;
            _memoryCashe = memoryCache;
        }

        public async Task<IEnumerable<City>> GetCitiesAsync(CancellationToken cancellationToken = default)
        {
            return await _decorated.GetCitiesAsync(cancellationToken);
        }

        public async Task<City> GetCityAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _decorated.GetCityAsync(slug, cancellationToken);
        }

        public async Task<IEnumerable<EventCategory>> GetEventCategoriesAsync(CancellationToken cancellationToken = default)
        {
            string key = $"categories";

            if (_memoryCashe.TryGetValue<IEnumerable<EventCategory>>(key, out var categories))
                return categories;

            categories = await _decorated.GetEventCategoriesAsync(cancellationToken);
            if (categories != null)
                _memoryCashe.Set(key, categories, TimeSpan.FromMinutes(5));

            return categories;
        }

        public async Task<GetEventsResult> GetEventsAsync(
            long since, 
            string location, 
            string orderBy, 
            string categories, 
            string fields, 
            int page,
            int pageSize,
            string textFormat,
            CancellationToken cancellationToken = default)
        {
            return await _decorated.GetEventsAsync(since, location, orderBy, categories, fields, page, pageSize, textFormat, cancellationToken);
        }
    }
}
