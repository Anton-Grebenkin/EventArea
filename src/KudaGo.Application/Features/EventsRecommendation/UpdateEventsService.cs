

using KudaGo.Application.Common.ApiClient;
using KudaGo.Application.Common.Data;
using KudaGo.Application.Common.Data.Entites;

namespace KudaGo.Application.Features.EventsRecommendation
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

            //var maxId = await _eventRepository.GetMaxEventId();

            var since = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var page = 1;
            var pageSize = 100;
            var eventsResult = await _kudaGoApiClient.GetEventsAsync(since, "msk", page: page, pageSize: pageSize);

            while (page * pageSize < eventsResult.Count)
            {
                foreach (var e in eventsResult.Results)
                {
                    var eventExists = await _eventRepository.EventExistsAsync(e.Id);
                    if (!eventExists)
                        await _eventRepository.AddEventAsync(MapEvent(e));
                }

                page++;
                eventsResult = await _kudaGoApiClient.GetEventsAsync(since, "msk", page: page);
            }

        }

        private Event MapEvent(Common.ApiClient.Dto.Event @event)
        {
            var result = new Event();

            result.Id = @event.Id;
            result.PublicationDate = DateTimeOffset.FromUnixTimeSeconds(@event.PublicationDate).UtcDateTime;
            result.Categories = @event.Categories;
            result.SiteUrl = @event.SiteUrl;
            result.Description = @event.Description;
            result.Title = @event.Title;
            result.Images = new List<ImageInfo>();
            foreach (var item in @event.Images)
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
