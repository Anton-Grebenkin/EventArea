

using MongoDB.Bson;

namespace KudaGo.Application.Data.Entites
{
    public class Configuration
    {
        public ObjectId Id { get; set; }
        public IEnumerable<string> RequiredStartingCommands { get; set; }
    }
}
