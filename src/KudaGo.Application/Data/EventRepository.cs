

using KudaGo.Application.Data.Entites;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Telegram.Bot.Types;

namespace KudaGo.Application.Data
{
    public interface IEventRepository
    {
        Task<Event> GetEventAsync(long id);
        Task AddEventAsync(Event @event);
        Task<long> GetMaxEventId();
        Task<IEnumerable<Event>> GetNotRecommendedAsync();
        Task<Event> UpdateEventAsync(Event e);
    }
    public class EventRepository : IEventRepository
    {
        private static readonly string _collectionName = "events";
        private readonly IMongoDatabase _db;
        public EventRepository(IMongoDatabase db)
        {
            _db = db;
        }
        public async Task<Event?> GetEventAsync(long id)
        {
            return await _db.GetCollection<Event>(_collectionName)
                 .AsQueryable()
                 .FirstOrDefaultAsync();
        }
        public async Task<Event> UpdateEventAsync(Event e)
        {
            return await _db.GetCollection<Event>(_collectionName)
                .FindOneAndReplaceAsync(p => p.Id == e.Id, e, new() { ReturnDocument = ReturnDocument.After });
        }

        public async Task<long> GetMaxEventId()
        {
            var count = await _db.GetCollection<Event>(_collectionName)
                  .AsQueryable().CountAsync();

            if (count == 0)
                return 0;

            return await _db.GetCollection<Event>(_collectionName)
                 .AsQueryable().MaxAsync(e => e.Id);
        }

        public async Task AddEventAsync(Event @event)
        {
            @event.AddDate = DateTime.UtcNow;
            await _db.GetCollection<Event>(_collectionName).InsertOneAsync(@event);
            return;
        }

        public async Task<IEnumerable<Event>> GetNotRecommendedAsync()
        {
            var events = new List<Event>();

            if (await _db.GetCollection<Event>(_collectionName).AsQueryable().AnyAsync())
                events = await _db.GetCollection<Event>(_collectionName)
               .AsQueryable()
               .Where(e => !e.Recommended)
               .ToListAsync();

            return events;         
        }
    }
}
