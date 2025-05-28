using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDBAPI.Data.Models;
using MongoDBAPI.Data.Services.Interfaces;

namespace MongoDBAPI.Data.Services
{
    public class TitleService : ITitleService
    {
        private readonly IMongoCollection<Title> _titles;
        public TitleService(IConfiguration config, IMongoClient client)
        {
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _titles = database.GetCollection<Title>("Title");
        }
        public async Task<List<Title>> GetTitles()
        {
            return await _titles.Find(_ => true)
                        .Limit(10)
                        .ToListAsync();
        }
    }
}
