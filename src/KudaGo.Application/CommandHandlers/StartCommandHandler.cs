using KudaGo.Application.Abstractions;
using KudaGo.Application.Attributes;
using KudaGo.Application.Data;
using Telegram.Bot;
using KudaGo.Application.ApiClient;
using Telegram.Bot.Types;
using KudaGo.Application.Extensions;
using KudaGo.Application.Messages;
using Telegram.Bot.Types.ReplyMarkups;

namespace KudaGo.Application.CommandHandlers
{
    [CommandType(CommandType.Start)]
    public class StartCommandHandler : IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IKudaGoApiClient _kudaGoClient;
        private readonly IUserRepository _userRepository;
        private readonly IMessageProvider _messageProvider;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IRedirectService _redirectService;
        public StartCommandHandler(
            ITelegramBotClient botClient,
            IKudaGoApiClient kudaGoClient,
            IUserRepository userRepository,
            IMessageProvider messageProvider,
            IConfigurationRepository configurationRepository,
            IRedirectService redirectService
            )
        {
            _botClient = botClient;
            _userRepository = userRepository;
            _kudaGoClient = kudaGoClient;
            _messageProvider = messageProvider;
            _configurationRepository = configurationRepository;
            _redirectService = redirectService;
        }
        public async Task HandleAsync(MessageContext updateContext, CancellationToken ct)
        {
            await _botClient.SendTypingActionAsync(updateContext.ChatId, ct);

            var userExists = await _userRepository.UserExistsAsync(updateContext.ChatId);
            if (userExists)
            {
                await CaseUserExists(updateContext, ct);
            }
            else
            {
                await CaseUserDoesNotExists(updateContext, ct);
            }

        }

        private async Task CaseUserExists(MessageContext updateContext, CancellationToken ct)
        {
           
        }

        private async Task CaseUserDoesNotExists(MessageContext updateContext, CancellationToken ct)
        {
            var messageData = await _messageProvider.WelcomeMessageAsync();

            await _botClient.SendMessageAsync(updateContext.ChatId, messageData, ct);

            await _botClient.SendTypingActionAsync(updateContext.ChatId, ct);

            var user = await _userRepository.AddUserAsync(new Data.Entites.User 
            { 
                Id = updateContext.ChatId,
                UserName = updateContext.User.Username,
                City = "msk"
            });

            await _redirectService.RedirectAsync(CommandType.Categories.GetCommandString(), updateContext, ct);
        }
    }
}
