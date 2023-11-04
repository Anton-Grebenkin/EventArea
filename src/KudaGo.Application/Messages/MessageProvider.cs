using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using KudaGo.Application.Abstractions;
using KudaGo.Application.ApiClient.Dto;
using KudaGo.Application.CommandHandlers;
using KudaGo.Application.Data;
using KudaGo.Application.Data.Entites;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KudaGo.Application.Messages
{
    public class MessageProvider : IMessageProvider
    {
        private readonly IMessageTemplateRepository _messageTemplateRepository;
        public MessageProvider(IMessageTemplateRepository messageTemplateRepository)
        {
            _messageTemplateRepository = messageTemplateRepository;
        }

        public async Task<MessageData> CitySelectedMessageAsync(string cyty, CallbackType callbackType)
        {
            var massegeTemplate = await _messageTemplateRepository.GetMessageTemplateAsync(MessageTemplateType.CitySelected);

            var text = $"{massegeTemplate.Text} {cyty}. Чтобы изменить выбор, используйте команду {CommandType.City.GetCommandString()}";

            var callbackData = new CallbackData
            {
                CallbackType = callbackType
            };

            return new MessageData(text);
        }

        public async Task<MessageData> CitySelectionMessageAsync(IEnumerable<City> cities, CallbackType callbackType)
        {
            var buttons = new List<List<InlineKeyboardButton>>();
            foreach (var c in cities)
            {
                var callbackData = new CallbackData
                { 
                    Data = c.Slug,
                    CallbackType = callbackType
                };

                buttons.Add(new List<InlineKeyboardButton> 
                {
                    InlineKeyboardButton.WithCallbackData(c.Name, callbackData.ToJsonString()) 
                });
            }

            InlineKeyboardMarkup inlineKeyboard = new(buttons);

            var messageTemplate = await _messageTemplateRepository.GetMessageTemplateAsync(MessageTemplateType.CitySelection);

            return new MessageData(messageTemplate.Text, inlineKeyboard);
        }

        public async Task<MessageData> CompleteSelectCategoriesMessageAsync()
        {
            var messageTemplate = await _messageTemplateRepository.GetMessageTemplateAsync(MessageTemplateType.CategoriesSelected);

            return new MessageData($"{messageTemplate.Text}. Чтобы изменить выбор, используйте команду {CommandType.Categories.GetCommandString()}");
        }

        public async Task<IEnumerable<InputMediaPhoto>> EventReccomendationMessageAsync(Data.Entites.Event @event)
        {
            var text = new StringBuilder();
            text.AppendLine(@event.Title);
            text.AppendLine();
            text.AppendLine(@event.Description);
            text.AppendLine();
            text.AppendLine(@event.SiteUrl);

            var media = new List<InputMediaPhoto>();
            var captionAdded = false;
            var urls = @event.Images.Select(i => i.Image).Take(5);
            foreach (string url in urls)
            {
                var file = InputFile.FromUri(url);

                InputMediaPhoto photo = new InputMediaPhoto(file);

                if (!captionAdded)
                    photo.Caption = text.ToString();

                media.Add(photo);

                captionAdded = true;
            }

            return media;
        }

        public async Task<MessageData> SelectCategoriesMessageAsync(IEnumerable<ItemSelection<EventCategory>> categories, int currentPage, bool nextPage, bool previosPage, CallbackType callbackType)
        {
            var buttons = new List<List<InlineKeyboardButton>>();

            foreach (var c in categories)
            {
                var buttonInfo = new SelectCategoriesButtonInfo 
                {
                    Slug = c.Value.Slug,
                    Action = SelectCategoriesButtonAction.SelectItem,
                    Page = currentPage
                };

                var callbackData = new CallbackData
                {
                    Data = buttonInfo.ToString(),
                    CallbackType = callbackType
                };


                buttons.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData($"{c.Value.Name} {c.SelectedString}", callbackData.ToJsonString())
                });
            }

            var navigation = new List<InlineKeyboardButton>();

            if (previosPage)
            {
                var previosPageButtonInfo = new SelectCategoriesButtonInfo
                {
                    Slug = null,
                    Action = SelectCategoriesButtonAction.PreviosPage,
                    Page = currentPage
                };
                var previosCallbackData = new CallbackData
                {
                    Data = previosPageButtonInfo.ToString(),
                    CallbackType = callbackType
                };
                navigation.Add(InlineKeyboardButton.WithCallbackData($"{Emoji.Previos} Назад", previosCallbackData.ToJsonString()));
            }

            if (nextPage)
            {
                var nextPageButtonInfo = new SelectCategoriesButtonInfo
                {
                    Slug = null,
                    Action = SelectCategoriesButtonAction.NextPage,
                    Page = currentPage
                };
                var nextPageCallbackData = new CallbackData
                {
                    Data = nextPageButtonInfo.ToString(),
                    CallbackType = callbackType
                };
                navigation.Add(InlineKeyboardButton.WithCallbackData($"Вперед {Emoji.Next}", nextPageCallbackData.ToJsonString()));
            }

            buttons.Add(navigation);

            var completeButtonInfo = new SelectCategoriesButtonInfo
            {
                Slug = null,
                Action = SelectCategoriesButtonAction.Complete,
                Page = currentPage
            };
            var completeCallbackData = new CallbackData
            {
                Data = completeButtonInfo.ToString(),
                CallbackType = callbackType
            };

            var complete = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData($"Подтвердить", completeCallbackData.ToJsonString())
            };

            buttons.Add(complete);

            InlineKeyboardMarkup inlineKeyboard = new(buttons);

            var messageTemplate = await _messageTemplateRepository.GetMessageTemplateAsync(MessageTemplateType.SelectCategories);

            return new MessageData(messageTemplate.Text, inlineKeyboard);
        }

        public async Task<MessageData> WelcomeMessageAsync()
        {
            var massegeTemplate = await _messageTemplateRepository.GetMessageTemplateAsync(MessageTemplateType.WelcomeMessage);

            return new MessageData(massegeTemplate.Text);
        }
    }
}
