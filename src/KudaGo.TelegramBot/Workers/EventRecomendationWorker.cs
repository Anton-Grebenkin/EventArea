
using KudaGo.Application.Features.EventsRecommendation;

namespace KudaGo.TelegramBot.Workers
{
    public class EventRecomendationWorker : BackgroundService
    {
        private readonly ILogger<EventRecomendationWorker> _logger;
        private readonly IEventRecommendationService _eventRecommendationService;
        public EventRecomendationWorker(ILogger<EventRecomendationWorker> logger, IEventRecommendationService eventRecommendationService)
        {
            _logger = logger;
            _eventRecommendationService = eventRecommendationService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _eventRecommendationService.RecommendEventsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
    }
}
