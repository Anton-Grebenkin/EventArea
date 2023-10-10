using KudaGo.Application.Data.Entites;
using MongoDB.Driver;
using Telegram.Bot.Types;

namespace KudaGo.Application.Data
{
    public interface IMessageTemplateRepository
    {
        Task<MessageTemplate> GetMessageTemplateAsync(MessageTemplateType messageTemplateType);
        Task<MessageTemplate> AddMessageTemplateAsync(MessageTemplate messageTemplate);
    }

    public class MessageTemplateRepository : IMessageTemplateRepository
    {
        private static readonly string _collectionName = "messages";
        private readonly IMongoDatabase _db;
        public MessageTemplateRepository(IMongoDatabase db)
        {
            _db = db;
        }
        public async Task<MessageTemplate> GetMessageTemplateAsync(MessageTemplateType messageTemplateType)
        {
            return await _db.GetCollection<MessageTemplate>(_collectionName)
               .Find(p => p.MessageTemplateType == messageTemplateType)
               .FirstOrDefaultAsync();
        }

        public async Task<MessageTemplate> AddMessageTemplateAsync(MessageTemplate messageTemplate)
        {
            await _db.GetCollection<MessageTemplate>(_collectionName).InsertOneAsync(messageTemplate);
            return messageTemplate;
        }


    }
}
