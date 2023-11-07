using KudaGo.Application.Common.Data.Entites;
using Microsoft.Extensions.Caching.Memory;

namespace KudaGo.Application.Common.Data
{
    public class CachedUserRepository : IUserRepository
    {
        private readonly UserRepository _decorated;
        private readonly IMemoryCache _memoryCashe;

        public CachedUserRepository(UserRepository userRepository, IMemoryCache memoryCashe) 
        {
            _decorated = userRepository;
            _memoryCashe = memoryCashe;
        }

        public async Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default)
        {
            user = await _decorated.AddUserAsync(user, cancellationToken);

            string key = $"user-{user.Id}";

            _memoryCashe.Set(key, user, TimeSpan.FromMinutes(5));

            return user;
        }

        public async Task<User> GetUserAsync(long userId, CancellationToken cancellationToken = default)
        {
            string key = $"user-{userId}";

            if (_memoryCashe.TryGetValue<User>(key, out var user))
                return user;

            user = await _decorated.GetUserAsync(userId, cancellationToken);
            if (user != null)
                _memoryCashe.Set(key, user, TimeSpan.FromMinutes(5));

            return user;
        }

        public async Task<IEnumerable<User>> GetUsersForEventReccomendation(IEnumerable<string> categories, CancellationToken cancellationToken = default)
        {
            return await _decorated.GetUsersForEventReccomendation(categories, cancellationToken);
        }

        public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            string key = $"user-{user.Id}";

            user = await _decorated.UpdateUserAsync(user, cancellationToken);

            _memoryCashe.Set(key, user, TimeSpan.FromMinutes(5));

            return user;
        }

        public async Task<bool> UserExistsAsync(long userId, CancellationToken cancellationToken = default)
        {
            string key = $"user-{userId}";
            if (_memoryCashe.TryGetValue(key, out var user))
                return true;

            return await _decorated.UserExistsAsync(userId, cancellationToken);
        }
    }
}
