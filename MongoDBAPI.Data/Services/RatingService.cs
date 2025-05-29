using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDBAPI.Data.Models;
using MongoDBAPI.Data.Services.Interfaces;

namespace MongoDBAPI.Data.Services
{
    public class RatingService : IRatingService
    {
        private readonly IMongoCollection<Rating> _ratings;
        public RatingService(IConfiguration config, IMongoClient client)
        {
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _ratings = database.GetCollection<Rating>("Rating");
        }
        public async Task<List<Rating>> GetRatings()
        {
            return await _ratings.Find(_ => true)
                        .Limit(20)
                        .ToListAsync();
        }
    }
}
