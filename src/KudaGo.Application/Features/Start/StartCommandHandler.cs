using Telegram.Bot;
using KudaGo.Application.Common.Abstractions;
using KudaGo.Application.Common.Attributes;
using KudaGo.Application.Common.Messages;
using KudaGo.Application.Common.ApiClient;
using KudaGo.Application.Common.Data;
using KudaGo.Application.Common.Extensions;

namespace KudaGo.Application.Features.Start
{
    [CommandType(CommandType.Start)]
    public class StartCommandHandler : IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IKudaGoApiClient _kudaGoClient;
        private readonly IUserRepository _userRepository;
        private readonly IMessageProvider _messageProvider;
        private readonly ICommandExecutor _redirectService;
        public StartCommandHandler(
            ITelegramBotClient botClient,
            IKudaGoApiClient kudaGoClient,
            IUserRepository userRepository,
            IMessageProvider messageProvider,
            ICommandExecutor redirectService
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
            await _botClient.SendTypingActionAsync(updateContext.ChatId, ct);

            var userExists = await _userRepository.UserExistsAsync(updateContext.ChatId, ct);
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
            var messageData = await _messageProvider.WelcomeMessageAsync(ct);

            await _botClient.SendMessageAsync(updateContext.ChatId, messageData, ct);
        }

        private async Task CaseUserDoesNotExists(MessageContext updateContext, CancellationToken ct)
        {
            var messageData = await _messageProvider.WelcomeMessageAsync(ct);

            await _botClient.SendMessageAsync(updateContext.ChatId, messageData, ct);

            var user = await _userRepository.AddUserAsync(new Common.Data.Entites.User
            {
                Id = updateContext.ChatId,
                UserName = updateContext.User.Username,
                City = "msk"
            },
            ct);

            await _redirectService.ExecuteNextCommandAsync(CommandType.Categories.GetCommandString(), updateContext, ct);
        }
    }
}
