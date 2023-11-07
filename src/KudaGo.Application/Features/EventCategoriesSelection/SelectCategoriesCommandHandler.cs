using KudaGo.Application.Common.Abstractions;
using KudaGo.Application.Common.ApiClient;
using KudaGo.Application.Common.ApiClient.Dto;
using KudaGo.Application.Common.Attributes;
using KudaGo.Application.Common.Data;
using KudaGo.Application.Common.Extensions;
using KudaGo.Application.Common.Messages;
using Telegram.Bot;

namespace KudaGo.Application.Features.EventCategoriesSelection
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
        public const int PAGE_SIZE = 5;
        private readonly IUserRepository _userRepository;
        private readonly ITelegramBotClient _botClient;
        private readonly IKudaGoApiClient _kudaGoApiClient;
        private readonly IMessageProvider _messageProvider;
        private readonly ICommandExecutor _commandExecutor;

        public SelectCategoriesCommandHandler(
            IUserRepository userRepository,
            ITelegramBotClient botClient,
            IKudaGoApiClient kudaGoApiClient,
            IMessageProvider messageProvider,
            ICommandExecutor commandExecutor
            )
        {
            _userRepository = userRepository;
            _botClient = botClient;
            _kudaGoApiClient = kudaGoApiClient;
            _messageProvider = messageProvider;
            _commandExecutor = commandExecutor;
        }
        public async Task HandleAsync(MessageContext updateContext, CancellationToken cancellationToken)
        {
            var userExists = await _userRepository.UserExistsAsync(updateContext.ChatId, cancellationToken);
            if (userExists)
            {
                await _botClient.SendTypingActionAsync(updateContext.ChatId, cancellationToken);

                var user = await _userRepository.GetUserAsync(updateContext.ChatId, cancellationToken);

                var categories = await _kudaGoApiClient.GetEventCategoriesAsync(cancellationToken);

                var categoriesCount = categories.Count();

                categories = categories.Take(PAGE_SIZE);

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

                var messageData = await _messageProvider.SelectCategoriesMessageAsync(selectionItems, 1, categoriesCount > 5, false, CallbackType.SelectCategories, cancellationToken);

                await _botClient.SendMessageAsync(updateContext.ChatId, messageData, cancellationToken);
            }
            else
                await _commandExecutor.ExecuteNextCommandAsync(CommandType.City.GetCommandString(), updateContext, cancellationToken);
        }
    }

    [CallbackType(CallbackType.SelectCategories)]
    public class SelectCategoriesCallbackHandler : IMessageHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ITelegramBotClient _botClient;
        private readonly IKudaGoApiClient _kudaGoApiClient;
        private readonly IMessageProvider _messageProvider;
        public SelectCategoriesCallbackHandler(
            IUserRepository userRepository,
            ITelegramBotClient botClient,
            IKudaGoApiClient kudaGoApiClient,
            IMessageProvider messageProvider
            )
        {
            _userRepository = userRepository;
            _botClient = botClient;
            _kudaGoApiClient = kudaGoApiClient;
            _messageProvider = messageProvider;
        }
        public async Task HandleAsync(MessageContext updateContext, CancellationToken cancellationToken)
        {
            var callbackData = SelectCategoriesButtonInfo.FromString(updateContext.CallbackData.Data);
            if (callbackData.Action == SelectCategoriesButtonAction.Complete)
            {
                var completeMessageData = await _messageProvider.CompleteSelectCategoriesMessageAsync(cancellationToken);
                await _botClient.EditMessageAsync(updateContext.ChatId, updateContext.MessageId, completeMessageData, cancellationToken);
                return;
            }

            var user = await _userRepository.GetUserAsync(updateContext.ChatId, cancellationToken);

            var categories = await _kudaGoApiClient.GetEventCategoriesAsync(cancellationToken);

            int page = callbackData.Page;

            switch (callbackData.Action)
            {
                case SelectCategoriesButtonAction.SelectItem:
                    if (user.PreferredEventCategories.Contains(callbackData.Slug))
                        user.PreferredEventCategories.Remove(callbackData.Slug);
                    else
                        user.PreferredEventCategories.Add(callbackData.Slug);

                    user = await _userRepository.UpdateUserAsync(user, cancellationToken);
                    break;
                case SelectCategoriesButtonAction.NextPage:
                    page++;
                    break;
                case SelectCategoriesButtonAction.PreviosPage:
                    page--;
                    break;
            }

            var categoriesCount = categories.Count();

            var take = SelectCategoriesCommandHandler.PAGE_SIZE;
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

            var messageData = await _messageProvider.SelectCategoriesMessageAsync(
                selectionItems, 
                page, 
                categoriesCount > skip + take, page > 1,
                CallbackType.SelectCategories,
                cancellationToken);

            await _botClient.EditMessageAsync(updateContext.ChatId, updateContext.MessageId, messageData, cancellationToken);
        }
    }
}
