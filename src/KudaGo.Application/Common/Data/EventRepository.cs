using KudaGo.Application.Common.Data.Entites;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Threading;


namespace KudaGo.Application.Common.Data
{
    public interface IEventRepository
    {
        Task<Event> GetEventAsync(long id, CancellationToken cancellationToken = default);
        Task<bool> EventExistsAsync(long id, CancellationToken cancellationToken = default);
        Task AddEventAsync(Event @event, CancellationToken cancellationToken = default);
        Task<long> GetMaxEventId(CancellationToken cancellationToken = default);
        Task<IEnumerable<Event>> GetNotRecommendedAsync(CancellationToken cancellationToken = default);
        Task<Event> UpdateEventAsync(Event e, CancellationToken cancellationToken = default);
    }
    public class EventRepository : IEventRepository
    {
        private static readonly string _collectionName = "events";
        private readonly IMongoDatabase _db;
        public EventRepository(IMongoDatabase db)
        {
            _db = db;
        }
        public async Task<Event?> GetEventAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _db.GetCollection<Event>(_collectionName)
                 .AsQueryable()
                 .Where(e => e.Id == id)
                 .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<Event> UpdateEventAsync(Event e, CancellationToken cancellationToken = default)
        {
            return await _db.GetCollection<Event>(_collectionName)
                .FindOneAndReplaceAsync(p => p.Id == e.Id, e, new() { ReturnDocument = ReturnDocument.After }, cancellationToken);
        }

        public async Task<long> GetMaxEventId(CancellationToken cancellationToken = default)
        {
            var count = await _db.GetCollection<Event>(_collectionName)
                  .AsQueryable().CountAsync(cancellationToken);

            if (count == 0)
                return 0;

            return await _db.GetCollection<Event>(_collectionName)
                 .AsQueryable().MaxAsync(e => e.Id, cancellationToken);
        }

        public async Task AddEventAsync(Event @event, CancellationToken cancellationToken = default)
        {
            @event.AddDate = DateTime.UtcNow;
            await _db.GetCollection<Event>(_collectionName).InsertOneAsync(@event, null, cancellationToken);
            return;
        }

        public async Task<IEnumerable<Event>> GetNotRecommendedAsync(CancellationToken cancellationToken = default)
        {
            var events = new List<Event>();

            if (await _db.GetCollection<Event>(_collectionName).AsQueryable().AnyAsync())
                events = await _db.GetCollection<Event>(_collectionName)
               .AsQueryable()
               .Where(e => !e.Recommended)
               .ToListAsync(cancellationToken);

            return events;         
        }

        public async Task<bool> EventExistsAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _db.GetCollection<Event>(_collectionName)
                 .AsQueryable()
                 .AnyAsync(e => e.Id == id, cancellationToken);
        }
    }
}
