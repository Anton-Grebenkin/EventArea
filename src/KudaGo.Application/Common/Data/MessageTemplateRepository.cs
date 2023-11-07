using KudaGo.Application.Common.Data.Entites;
using MongoDB.Driver;

namespace KudaGo.Application.Common.Data
{
    public interface IMessageTemplateRepository
    {
        Task<MessageTemplate> GetMessageTemplateAsync(MessageTemplateType messageTemplateType, CancellationToken cancellationToken = default);
        Task<MessageTemplate> AddMessageTemplateAsync(MessageTemplate messageTemplate, CancellationToken cancellationToken = default);
    }

    public class MessageTemplateRepository : IMessageTemplateRepository
    {
        private static readonly string _collectionName = "messages";
        private readonly IMongoDatabase _db;
        public MessageTemplateRepository(IMongoDatabase db)
        {
            _db = db;
        }
        public async Task<MessageTemplate> GetMessageTemplateAsync(MessageTemplateType messageTemplateType, CancellationToken cancellationToken = default)
        {
            return await _db.GetCollection<MessageTemplate>(_collectionName)
               .Find(p => p.MessageTemplateType == messageTemplateType)
               .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<MessageTemplate> AddMessageTemplateAsync(MessageTemplate messageTemplate, CancellationToken cancellationToken = default)
        {
            await _db.GetCollection<MessageTemplate>(_collectionName).InsertOneAsync(messageTemplate, null, cancellationToken);
            return messageTemplate;
        }


    }
}
