using KudaGo.Application.ApiClient;
using KudaGo.Application.Data;
using KudaGo.Application.Data.Entites;

namespace KudaGo.Application.Services
{
    public interface IUpdateEventsService
    {
        Task UpdateEvents();
    }
    public class UpdateEventsService : IUpdateEventsService
    {
        private readonly IKudaGoApiClient _kudaGoApiClient;
        private readonly IEventRepository _eventRepository;
        public UpdateEventsService(IKudaGoApiClient kudaGoApiClient, IEventRepository eventRepository)
        {
            _kudaGoApiClient = kudaGoApiClient;
            _eventRepository = eventRepository;
        }
        public async Task UpdateEvents()
        {

            var maxId = await _eventRepository.GetMaxEventId();

            var since = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var page = 1;
            var pageSize = 100;
            var eventsResult = await _kudaGoApiClient.GetEventsAsync(since, "msk", page: page, pageSize: pageSize);

            while (eventsResult.Results.Any(e => e.Id > maxId))
            {
                var eventsForSave = eventsResult.Results.Where(e => e.Id > maxId);
                foreach ( var e in eventsForSave)
                {
                    await _eventRepository.AddEventAsync(MapEvent(e));
                }

                if (page * pageSize < eventsResult.Count)
                {
                    page++;
                    eventsResult = await _kudaGoApiClient.GetEventsAsync(since, "msk", page: page);
                }
                else
                {
                    break;
                }
                    
            }

        }

        private Event MapEvent(ApiClient.Dto.Event @event)
        {
            var result = new Event();

            result.Id = @event.Id;
            result.PublicationDate = DateTimeOffset.FromUnixTimeSeconds(@event.PublicationDate).UtcDateTime;
            result.Categories = @event.Categories;
            result.SiteUrl = @event.SiteUrl;
            result.Description = @event.Description;
            result.Title = @event.Title;
            result.Images = new List<ImageInfo>();
            foreach( var item in @event.Images)
            {
                result.Images.Add(new ImageInfo
                {
                    Image = item.Image
                });
            }

            return result;
        }
    }
}
