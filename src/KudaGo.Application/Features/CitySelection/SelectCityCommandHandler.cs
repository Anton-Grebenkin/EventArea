using Telegram.Bot;
using KudaGo.Application.Common.Abstractions;
using KudaGo.Application.Common.Attributes;
using KudaGo.Application.Common.Messages;
using KudaGo.Application.Common.ApiClient;
using KudaGo.Application.Common.Data;
using KudaGo.Application.Common.Extensions;

namespace KudaGo.Application.Features.CitySelection
{
    [CommandType(CommandType.City)]
    [NotUsed]
    public class SelectCityCommandHandler : IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IKudaGoApiClient _kudaGoClient;
        private readonly IUserRepository _userRepository;
        private readonly IMessageProvider _messageProvider;
        private readonly ICommandExecutor _commandExecutor;
        public SelectCityCommandHandler(
            ITelegramBotClient botClient,
            IKudaGoApiClient kudaGoClient,
            IUserRepository userRepository,
            IMessageProvider messageProvider,
            ICommandExecutor commandExecutor
            )
        {
            _botClient = botClient;
            _userRepository = userRepository;
            _kudaGoClient = kudaGoClient;
            _messageProvider = messageProvider;
            _commandExecutor = commandExecutor;
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
                await _commandExecutor.ExecuteNextCommandAsync(CommandType.Start.GetCommandString(), updateContext, ct);
        }
    }
    [CallbackType(CallbackType.CitySelection)]
    public class CitySelectionCallbackHandler : IMessageHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ITelegramBotClient _botClient;
        private readonly IKudaGoApiClient _kudaGoApiClient;
        private readonly IMessageProvider _messageProvider;
        private readonly ICommandExecutor _redirectService;
        public CitySelectionCallbackHandler(
            IUserRepository userRepository,
            ITelegramBotClient botClient,
            IKudaGoApiClient kudaGoApiClient,
            IMessageProvider messageProvider,
            ICommandExecutor redirectService
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
                await _redirectService.ExecuteNextCommandAsync(CommandType.Categories.GetCommandString(), updateContext, cancellationToken);

        }
    }
}
