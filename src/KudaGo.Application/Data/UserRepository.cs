using KudaGo.Application.Data.Entites;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace KudaGo.Application.Data
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<User> GetUserAsync(long userId);
        Task<User> UpdateUserAsync(User user);
        Task<bool> UserExistsAsync(long userId);
        Task<IEnumerable<User>> GetUsersForEventReccomendation(IEnumerable<string> categories);

    }
    public class UserRepository : IUserRepository
    {
        private static readonly string _collectionName = "users";
        private readonly IMongoDatabase _db;
        public UserRepository(IMongoDatabase db)
        {
            _db = db;
        }
        public async Task<User> AddUserAsync(User user)
        {
            await _db.GetCollection<User>(_collectionName).InsertOneAsync(user);
            return user;
        }

        public async Task<User> GetUserAsync(long userId)
        {
            return await _db.GetCollection<User>(_collectionName)
                .Find(p => p.Id == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetUsersForEventReccomendation(IEnumerable<string> categories)
        {
            return await _db.GetCollection<User>(_collectionName)
                .AsQueryable()
                .Where(u => u.PreferredEventCategories.Any(c => categories.Contains(c)) || !u.PreferredEventCategories.Any() && u.RecommendEvents)
                .ToListAsync();
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            return await _db.GetCollection<User>(_collectionName)
                .FindOneAndReplaceAsync(p => p.Id == user.Id, user, new() { ReturnDocument = ReturnDocument.After });
        }

        public async Task<bool> UserExistsAsync(long userId)
        {
            return await _db.GetCollection<User>(_collectionName).Find(p => p.Id == userId).AnyAsync();
        }

    }
}
