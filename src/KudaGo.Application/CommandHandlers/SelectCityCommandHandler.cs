using KudaGo.Application.Abstractions;
using KudaGo.Application.ApiClient;
using KudaGo.Application.Attributes;
using KudaGo.Application.Data;
using Telegram.Bot.Types;
using Telegram.Bot;
using KudaGo.Application.Extensions;
using KudaGo.Application.Messages;
using KudaGo.Application.Services;

namespace KudaGo.Application.CommandHandlers
{
    [CommandType(CommandType.City)]
    [NotUsed]
    public class SelectCityCommandHandler : IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IKudaGoApiClient _kudaGoClient;
        private readonly IUserRepository _userRepository;
        private readonly IMessageProvider _messageProvider;
        private readonly IRedirectService _redirectService;
        public SelectCityCommandHandler(
            ITelegramBotClient botClient,
            IKudaGoApiClient kudaGoClient,
            IUserRepository userRepository,
            IMessageProvider messageProvider,
            IRedirectService redirectService
            )
        {
            _botClient = botClient;
            _userRepository = userRepository;
            _kudaGoClient = kudaGoClient;
            _messageProvider = messageProvider;
            _redirectService = redirectService;
        }
        public async Task HandleAsync(MessageContext updateContext, CancellationToken ct)
        {
            var userExists = await _userRepository.UserExistsAsync(updateContext.ChatId);
            if (userExists)
            {
                await _botClient.SendTypingActionAsync(updateContext.ChatId, ct);

                var cities = await _kudaGoClient.GetCitiesAsync();

                var messageData = await _messageProvider.CitySelectionMessageAsync(cities, CallbackType.CitySelection);

                await _botClient.SendMessageAsync(updateContext.ChatId, messageData, ct);
            }
            else
                await _redirectService.RedirectAsync(CommandType.Start.GetCommandString(), updateContext, ct);
        }
    }
    [CallbackType(CallbackType.CitySelection)]
    public class CitySelectionCallbackHandler : IMessageHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ITelegramBotClient _botClient;
        private readonly IKudaGoApiClient _kudaGoApiClient;
        private readonly IMessageProvider _messageProvider;
        private readonly IRedirectService _redirectService;
        public CitySelectionCallbackHandler(
            IUserRepository userRepository,
            ITelegramBotClient botClient,
            IKudaGoApiClient kudaGoApiClient,
            IMessageProvider messageProvider,
            IRedirectService redirectService
            )
        {
            _userRepository = userRepository;
            _botClient = botClient;
            _kudaGoApiClient = kudaGoApiClient;
            _messageProvider = messageProvider;
            _redirectService = redirectService;
        }
        public async Task HandleAsync(MessageContext updateContext, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserAsync(updateContext.ChatId);

            var needRedirect = user.City == null;
            
            var city = await _kudaGoApiClient.GetCityAsync(updateContext.CallbackData.Data);

            user.City = city.Slug;

            await _userRepository.UpdateUserAsync(user);

            var successMessage = await _messageProvider.CitySelectedMessageAsync(city.Name, CallbackType.ChangeCity);

            await _botClient.EditMessageAsync(updateContext.ChatId, updateContext.MessageId, successMessage, cancellationToken);

            if (needRedirect)
                await _redirectService.RedirectAsync(CommandType.Categories.GetCommandString(), updateContext, cancellationToken);

        }
    }
}
