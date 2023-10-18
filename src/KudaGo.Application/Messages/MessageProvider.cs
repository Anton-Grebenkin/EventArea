using KudaGo.Application.Abstractions;
using KudaGo.Application.ApiClient.Dto;
using KudaGo.Application.Data;
using KudaGo.Application.Data.Entites;
using Newtonsoft.Json;
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

        public async Task<MessageData> SitySelectionMessageAsync(IEnumerable<City> cities, string nextCommand)
        {
            var buttons = new List<List<InlineKeyboardButton>>();
            foreach (var c in cities)
            {
                var callbackData = new CallbackData
                { 
                    CallbackType = CallbackType.CitySelection, 
                    Data = c.Slug,
                    NextCommand = nextCommand
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

        public async Task<MessageData> WelcomeMessageAsync()
        {
            var massegeTemplate = await _messageTemplateRepository.GetMessageTemplateAsync(MessageTemplateType.WelcomeMessage);

            return new MessageData(massegeTemplate.Text);
        }
    }
}
