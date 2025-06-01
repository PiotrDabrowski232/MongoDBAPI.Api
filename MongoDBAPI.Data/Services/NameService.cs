using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBAPI.Data.Models;
using MongoDBAPI.Data.Models.Name;
using MongoDBAPI.Data.Services.Interfaces;
using System.Diagnostics;

namespace MongoDBAPI.Data.Services
{
    public class NameService : INameService
    {
        private readonly IMongoCollection<NameObj> _names;
        public NameService(IConfiguration config, IMongoClient client)
        {
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _names = database.GetCollection<NameObj>("Name2");
        }
        public async Task<List<NameObj>> GetNames()
        {
            return await _names.Find(_ => true)
                        .Limit(20)
                        .ToListAsync();
        }
        public async Task<List<CountResult>> NameCount(CancellationToken ct)
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
            var countResult = await _names.Aggregate<BsonDocument>(new[] { groupStage }).FirstOrDefaultAsync(ct);
            stopwatch1.Stop();

            result.Add(new CountResult { Method = "Aggregate", DocumentsCount = countResult["count"].AsInt32, Time = stopwatch1.ElapsedMilliseconds });

            //B
            var indexKeys = Builders<NameObj>.IndexKeys.Ascending(i => i.Id);
            var indexModel = new CreateIndexModel<NameObj>(indexKeys);
            await _names.Indexes.CreateOneAsync(indexModel);

            var stopwatch2 = Stopwatch.StartNew();
            var countResultIndexed = await _names.Aggregate<BsonDocument>(new[] { groupStage }).FirstOrDefaultAsync(ct);
            stopwatch2.Stop();

            result.Add(new CountResult { Method = "Aggregate + Index", DocumentsCount = countResultIndexed["count"].AsInt32, Time = stopwatch2.ElapsedMilliseconds });

            var stopwatch3 = Stopwatch.StartNew();
            var countWithFind = await _names.CountDocumentsAsync(new BsonDocument(), cancellationToken: ct);
            stopwatch3.Stop();

            result.Add(new CountResult { Method = "Find", DocumentsCount = countWithFind, Time = stopwatch3.ElapsedMilliseconds });

            return result;
        }

        public async Task<(int TotalCount, List<Models.Name.Name.NameDto> Top5)> FindPeopleBySurnameAsync()
        {
            var filter = Builders<NameObj>.Filter.Or(
                Builders<NameObj>.Filter.Regex("primaryName", new BsonRegularExpression("Fonda")),
                Builders<NameObj>.Filter.Regex("primaryName", new BsonRegularExpression("Coppola"))
            );

            var matchingDocs = await _names.Find(filter).ToListAsync();
            int totalCount = matchingDocs.Count();

            var top5 = matchingDocs
                .Take(5)
                .Select(d => new Models.Name.Name.NameDto
                {
                    PrimaryName = d.PrimaryName ?? "",
                    PrimaryProfession = d.PrimaryProfession ?? ""
                })
                .ToList();

            return (totalCount, top5);
        }


        public async Task<IndexListResult> CreateBirthYearDescendingIndexAndListAsync()
        {
            var indexModel = new CreateIndexModel<NameObj>(
                Builders<NameObj>.IndexKeys.Descending(n => n.BirthYear),
                new CreateIndexOptions { Name = "birthYear_desc" });

            await _names.Indexes.CreateOneAsync(indexModel);

            var indexCursor = await _names.Indexes.ListAsync();
            var rawList = await indexCursor.ToListAsync();
            var indexList = rawList
                .Select(bson => bson.ToDictionary(
                    e => e.Name,
                    e => BsonTypeMapper.MapToDotNetValue(e.Value)))
                .ToList();

            return new IndexListResult
            {
                IndexCount = indexList.Count,
                Indexes = indexList
            };
        }

    }
}
