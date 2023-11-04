using KudaGo.Application.Abstractions;
using KudaGo.Application.ApiClient;
using KudaGo.Application.ApiClient.Dto;
using KudaGo.Application.Attributes;
using KudaGo.Application.Data;
using KudaGo.Application.Extensions;
using KudaGo.Application.Messages;
using Telegram.Bot;

namespace KudaGo.Application.CommandHandlers
{
    public enum SelectCategoriesButtonAction
    {
        SelectItem = 0,
        NextPage = 1,
        PreviosPage = 2,
        Complete = 3
    }
    public record SelectCategoriesButtonInfo 
    {
        public string Slug { get; init; }
        public SelectCategoriesButtonAction Action { get; init; }
        public int Page { get; init; }

        public static SelectCategoriesButtonInfo FromString(string input)
        {
            var values = input.Split('|');

            return new SelectCategoriesButtonInfo
            {
                Slug = values[0],
                Action = Enum.Parse<SelectCategoriesButtonAction>(values[1]),
                Page = int.Parse(values[2])
            };
        }

        public override string ToString()
        {
            return $"{Slug}|{Action}|{Page}";
        }
    } 

    [CommandType(CommandType.Categories)]
    public class SelectCategoriesCommandHandler : IMessageHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ITelegramBotClient _botClient;
        private readonly IKudaGoApiClient _kudaGoApiClient;
        private readonly IMessageProvider _messageProvider;
        private readonly IRedirectService _redirectService;

        public SelectCategoriesCommandHandler(
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
            var userExists = await _userRepository.UserExistsAsync(updateContext.ChatId);
            if (userExists)
            {
                await _botClient.SendTypingActionAsync(updateContext.ChatId, cancellationToken);

                var user = await _userRepository.GetUserAsync(updateContext.ChatId);

                var categories = await _kudaGoApiClient.GetEventCategoriesAsync();

                var categoriesCount = categories.Count();

                categories = categories.Take(5);

                var selectionItems = new List<ItemSelection<EventCategory>>();
                foreach (var category in categories)
                {
                    var selected = user.PreferredEventCategories.Contains(category.Slug);
                    selectionItems.Add(new ItemSelection<EventCategory>
                    {
                        Selected = selected,
                        Value = category
                    });
                }

                var messageData = await _messageProvider.SelectCategoriesMessageAsync(selectionItems, 1, categoriesCount > 5, false, CallbackType.SelectCategories);

                await _botClient.SendMessageAsync(updateContext.ChatId, messageData, cancellationToken);
            }
            else
                await _redirectService.RedirectAsync(CommandType.City.GetCommandString(), updateContext, cancellationToken);
        }
    }

    [CallbackType(CallbackType.SelectCategories)]
    public class SelectCategoriesCallbackHandler : IMessageHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ITelegramBotClient _botClient;
        private readonly IKudaGoApiClient _kudaGoApiClient;
        private readonly IMessageProvider _messageProvider;
        private readonly IRedirectService _redirectService;
        public SelectCategoriesCallbackHandler(
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
            var callbackData = SelectCategoriesButtonInfo.FromString(updateContext.CallbackData.Data);
            if (callbackData.Action == SelectCategoriesButtonAction.Complete)
            {
                var completeMessageData = await _messageProvider.CompleteSelectCategoriesMessageAsync();
                await _botClient.EditMessageAsync(updateContext.ChatId, updateContext.MessageId, completeMessageData, cancellationToken);
                return;
            }

            var user = await _userRepository.GetUserAsync(updateContext.ChatId);

            var categories = await _kudaGoApiClient.GetEventCategoriesAsync();

            int page = callbackData.Page;

            switch (callbackData.Action)
            {
                case SelectCategoriesButtonAction.SelectItem:
                    if (user.PreferredEventCategories.Contains(callbackData.Slug))
                        user.PreferredEventCategories.Remove(callbackData.Slug);
                    else
                        user.PreferredEventCategories.Add(callbackData.Slug);

                    user = await _userRepository.UpdateUserAsync(user);
                    break;
                case SelectCategoriesButtonAction.NextPage:
                    page++;
                    break;
                case SelectCategoriesButtonAction.PreviosPage:
                    page--;
                    break;
            }

            var categoriesCount = categories.Count();

            var take = 5;
            var skip = (page - 1) * take;
            categories = categories
                .Skip(skip)
                .Take(take)
                .ToList();

            var selectionItems = new List<ItemSelection<EventCategory>>();
            foreach (var category in categories)
            {
                var selected = user.PreferredEventCategories.Contains(category.Slug);
                selectionItems.Add(new ItemSelection<EventCategory>
                {
                    Selected = selected,
                    Value = category
                });
            }

            var messageData = await _messageProvider.SelectCategoriesMessageAsync(selectionItems, page, categoriesCount > skip + take, page > 1, CallbackType.SelectCategories);

            await _botClient.EditMessageAsync(updateContext.ChatId, updateContext.MessageId, messageData, cancellationToken);
        }
    }
}
