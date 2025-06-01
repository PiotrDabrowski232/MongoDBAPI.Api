using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBAPI.Data.Models;
using MongoDBAPI.Data.Models.Rating;
using MongoDBAPI.Data.Services.Interfaces;
using System.Diagnostics;
using System.Xml.Linq;

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

        public async Task<List<CountResult>> RatingCount(CancellationToken ct)
        {
            var result = new List<CountResult>();

            var groupStage = new BsonDocument
            {
                {
                    "$group",
                    new BsonDocument
                    {
                        { "_id", BsonNull.Value },
                        { "count", new BsonDocument("$sum", 1) }
                    }
                }
            };

            //A
            var stopwatch1 = Stopwatch.StartNew();
            var countResult = await _ratings.Aggregate<BsonDocument>(new[] { groupStage }).FirstOrDefaultAsync(ct);
            stopwatch1.Stop();

            result.Add(new CountResult { Method = "Aggregate", DocumentsCount = countResult["count"].AsInt32, Time = stopwatch1.ElapsedMilliseconds });

            //B
            var indexKeys = Builders<Rating>.IndexKeys.Ascending(i => i.Id);
            var indexModel = new CreateIndexModel<Rating>(indexKeys);
            await _ratings.Indexes.CreateOneAsync(indexModel);

            var stopwatch2 = Stopwatch.StartNew();
            var countResultIndexed = await _ratings.Aggregate<BsonDocument>(new[] { groupStage }).FirstOrDefaultAsync(ct);
            stopwatch2.Stop();

            result.Add(new CountResult { Method = "Aggregate + Index", DocumentsCount = countResultIndexed["count"].AsInt32, Time = stopwatch2.ElapsedMilliseconds });

            var stopwatch3 = Stopwatch.StartNew();
            var countWithFind = await _ratings.CountDocumentsAsync(new BsonDocument(), cancellationToken: ct);
            stopwatch3.Stop();

            result.Add(new CountResult { Method = "Find", DocumentsCount = countWithFind, Time = stopwatch3.ElapsedMilliseconds });

            return result;
        }
    }
}
