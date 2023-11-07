using KudaGo.Application.Common.Data.Entites;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace KudaGo.Application.Common.Data
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default);
        Task<User> GetUserAsync(long userId, CancellationToken cancellationToken = default);
        Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> UserExistsAsync(long userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetUsersForEventReccomendation(IEnumerable<string> categories, CancellationToken cancellationToken = default);

    }
    public class UserRepository : IUserRepository
    {
        private static readonly string _collectionName = "users";
        private readonly IMongoDatabase _db;
        public UserRepository(IMongoDatabase db)
        {
            _db = db;
        }
        public async Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default)
        {
            await _db.GetCollection<User>(_collectionName).InsertOneAsync(user, null, cancellationToken);
            return user;
        }

        public async Task<User> GetUserAsync(long userId, CancellationToken cancellationToken = default)
        {
            return await _db.GetCollection<User>(_collectionName)
                .Find(p => p.Id == userId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetUsersForEventReccomendation(IEnumerable<string> categories, CancellationToken cancellationToken = default)
        {
            return await _db.GetCollection<User>(_collectionName)
                .AsQueryable()
                .Where(u => u.PreferredEventCategories.Any(c => categories.Contains(c)) || !u.PreferredEventCategories.Any() && u.RecommendEvents)
                .ToListAsync(cancellationToken);
        }

        public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            return await _db.GetCollection<User>(_collectionName)
                .FindOneAndReplaceAsync(p => p.Id == user.Id, user, new() { ReturnDocument = ReturnDocument.After }, cancellationToken);
        }

        public async Task<bool> UserExistsAsync(long userId, CancellationToken cancellationToken = default)
        {
            return await _db.GetCollection<User>(_collectionName).Find(p => p.Id == userId).AnyAsync(cancellationToken);
        }

    }
}
