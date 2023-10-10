using KudaGo.Application.Abstractions;
using KudaGo.Application.Attributes;
using KudaGo.Application.Data;
using Telegram.Bot;
using KudaGo.Application.ApiClient;
using Telegram.Bot.Types;
using KudaGo.Application.Extensions;

namespace KudaGo.Application.CommandHandlers
{
    [CommandName("/start")]
    public class StartCommandHandler : ICommandHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IKudaGoApiClient _kudaGoClient;
        private readonly IUserRepository _userRepository;
        private readonly IMessageProvider _messageProvider;
        private readonly IRedirectService _redirectService;
        public StartCommandHandler(
            ITelegramBotClient botClient, 
            IKudaGoApiClient kudaGoClient, 
            IUserRepository userRepository, 
            IMessageProvider messageProvider, 
            IRedirectService redirectService) 
        {
            _botClient = botClient;
            _userRepository = userRepository;
            _kudaGoClient = kudaGoClient;
            _messageProvider = messageProvider;
            _redirectService = redirectService;
        }
        public async Task HandleAsync(MessageContext messageContext, CancellationToken ct)
        {
            await _botClient.SendTypingActionAsync(messageContext.ContextData.ChatId, ct);

            var userExists = await _userRepository.UserExistsAsync(messageContext.ContextData.ChatId);
            if (userExists)
            {
                await CaseUserExists(messageContext.Update, ct);
            }
            else
            {
                await CaseUserDoesNotExists(messageContext.Update, ct);
            }

        }

        private async Task CaseUserExists(Message message, CancellationToken ct)
        {

        }

        private async Task CaseUserDoesNotExists(Message message, CancellationToken ct)
        {
            var messageData = await _messageProvider.WelcomeMessageAsync();

            await _botClient.SendMessageAsync(message.Chat.Id, messageData, ct);

            await _botClient.SendTypingActionAsync(message.Chat.Id, ct);

            await _userRepository.AddUserAsync(new Data.Entites.User { Id = message.Chat.Id, UserName = message.Chat.Username });

            await _redirectService.RedirectAsync("/city", message, ct);
        }
    }
}
