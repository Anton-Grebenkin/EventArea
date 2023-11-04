
using KudaGo.Application.Abstractions;
using KudaGo.Application.Data;
using KudaGo.Application.Extensions;
using Telegram.Bot;

namespace KudaGo.Application.Services
{
    public interface IEventRecommendationService
    {
        Task RecommendEventsAsync();
    }
    public class EventRecommendationService : IEventRecommendationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ITelegramBotClient _botClient;
        private readonly IMessageProvider _messageProvider;
        public EventRecommendationService(
            IUserRepository userRepository, 
            IEventRepository eventRepository, 
            ITelegramBotClient botClient, 
            IMessageProvider messageProvider
            )
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _botClient = botClient;
            _messageProvider = messageProvider;
        }
        public async Task RecommendEventsAsync()
        {
            var events = await _eventRepository.GetNotRecommendedAsync();
            foreach(var e in events)
            {
                var message = await _messageProvider.EventReccomendationMessageAsync(e);
                var users = await _userRepository.GetUsersWithAnyCategoryAsync(e.Categories);
                foreach (var user in users)
                {
                    await _botClient.SendMediaGroupAsync(user.Id, message);
                }

                e.Recommended = true;
                await _eventRepository.UpdateEventAsync(e);
            }
        }
    }
}
