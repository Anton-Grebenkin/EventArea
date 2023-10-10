using KudaGo.Application.Data.Entites;
using MongoDB.Driver;

namespace KudaGo.Application.Data
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<IEnumerable<string>> GetRequiredCommandsAsync(long userId);
        Task<User> GetUserAsync(long userId);
        Task<User> UpdateUserAsync(User user);
        Task<bool> UserExistsAsync(long userId);

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

        public async Task<IEnumerable<string>> GetRequiredCommandsAsync(long userId)
        {
            return (await _db.GetCollection<User>(_collectionName)
                .Find(p => p.Id == userId)
                .FirstOrDefaultAsync()).RequiredCommands;
        }

        public async Task<User> GetUserAsync(long userId)
        {
            return await _db.GetCollection<User>(_collectionName)
                .Find(p => p.Id == userId)
                .FirstOrDefaultAsync();
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
