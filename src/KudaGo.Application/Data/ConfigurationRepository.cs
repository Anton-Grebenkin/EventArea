using KudaGo.Application.Data.Entites;
using MongoDB.Driver;

namespace KudaGo.Application.Data
{
    public interface IConfigurationRepository
    {
        Task<Configuration> GetConfigurationAsync();
    }
    public class ConfigurationRepository : IConfigurationRepository
    {
        private static readonly string _collectionName = "configuration";
        private readonly IMongoDatabase _db;
        public ConfigurationRepository(IMongoDatabase db)
        {
            _db = db;
        }
        public async Task<Configuration> GetConfigurationAsync()
        {
            return await _db.GetCollection<Configuration>(_collectionName)
                .AsQueryable()
                .FirstOrDefaultAsync();
        }
    }
}
