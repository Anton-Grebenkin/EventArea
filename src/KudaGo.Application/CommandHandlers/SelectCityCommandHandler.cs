using KudaGo.Application.Abstractions;
using KudaGo.Application.ApiClient;
using KudaGo.Application.Attributes;
using KudaGo.Application.Data;
using Telegram.Bot.Types;
using Telegram.Bot;
using KudaGo.Application.Extensions;
using KudaGo.Application.Messages;

namespace KudaGo.Application.CommandHandlers
{
    [CommandName("/city")]
    public class SelectCityCommandHandler : ICommandHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IKudaGoApiClient _kudaGoClient;
        private readonly IUserRepository _userRepository;
        private readonly IMessageProvider _messageProvider;
        public SelectCityCommandHandler(
            ITelegramBotClient botClient,
            IKudaGoApiClient kudaGoClient,
            IUserRepository userRepository,
            IMessageProvider messageProvider
            )
        {
            _botClient = botClient;
            _userRepository = userRepository;
            _kudaGoClient = kudaGoClient;
            _messageProvider = messageProvider;
        }
        public async Task HandleAsync(Message message, CancellationToken ct)
        {
            var userExists = await _userRepository.UserExistsAsync(message.Chat.Id);
            if (userExists)
            {
                await _botClient.SendTypingActionAsync(message.Chat.Id, ct);

                var cities = await _kudaGoClient.GetCitiesAsync();

                var messageData = await _messageProvider.SitySelectionMessageAsync(cities, "/categories");

                await _botClient.SendMessageAsync(message.Chat.Id, messageData, ct);
            }
        }
    }
}
