using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDBAPI.Data.Models;
using MongoDBAPI.Data.Services.Interfaces;

namespace MongoDBAPI.Data.Services
{
    public class NameService : INameService
    {
        private readonly IMongoCollection<Name> _names;
        public NameService(IConfiguration config, IMongoClient client)
        {
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _names = database.GetCollection<Name>("Name");
        }
        public async Task<List<Name>> GetNames()
        {
            return await _names.Find(_ => true)
                        .Limit(20)
                        .ToListAsync();
        }
    }
}
