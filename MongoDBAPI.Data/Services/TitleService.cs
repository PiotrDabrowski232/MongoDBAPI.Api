using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBAPI.Data.Models;
using MongoDBAPI.Data.Models.Rating;
using MongoDBAPI.Data.Models.Title;
using MongoDBAPI.Data.Services.Interfaces;
using System.Diagnostics;

namespace MongoDBAPI.Data.Services
{
    public class TitleService : ITitleService
    {
        private readonly IMongoCollection<Title> _titles;
        private readonly IMongoCollection<Rating> _ratings;

        public TitleService(IConfiguration config, IMongoClient client)
        {
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _ratings = database.GetCollection<Rating>("Rating");
            _titles = database.GetCollection<Title>("Title3");
        }

        public async Task<List<Title>> GetTitles()
        {
            return await _titles.Find(_ => true)
                        .Limit(20)
                        .ToListAsync();
        }

        public async Task<List<CountResult>> TitleCount(CancellationToken ct)
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
            var countResult = await _titles.Aggregate<BsonDocument>(new[] { groupStage }).FirstOrDefaultAsync(ct);
            stopwatch1.Stop();

            result.Add(new CountResult { Method = "Aggregate", DocumentsCount = countResult["count"].AsInt32, Time = stopwatch1.ElapsedMilliseconds });

            //B
            var indexKeys = Builders<Title>.IndexKeys.Ascending(i => i.Id);
            var indexModel = new CreateIndexModel<Title>(indexKeys);
            await _titles.Indexes.CreateOneAsync(indexModel);

            var stopwatch2 = Stopwatch.StartNew();
            var countResultIndexed = await _titles.Aggregate<BsonDocument>(new[] { groupStage }).FirstOrDefaultAsync(ct);
            stopwatch2.Stop();

            result.Add(new CountResult { Method = "Aggregate + Index", DocumentsCount = countResultIndexed["count"].AsInt32, Time = stopwatch2.ElapsedMilliseconds });

            var stopwatch3 = Stopwatch.StartNew();
            var countWithFind = await _titles.CountDocumentsAsync(new BsonDocument(), cancellationToken: ct);
            stopwatch3.Stop();

            result.Add(new CountResult { Method = "Find", DocumentsCount = countWithFind, Time = stopwatch3.ElapsedMilliseconds });

            return result;
        }

        public async Task<(List<TitleDto> Results, long TotalCount)> GetFilteredTitlesAsync(CancellationToken ct)
        {
            var pipelineWithLimit = new[]
                {
                    new BsonDocument("$addFields", new BsonDocument
                    {
                        { "startYearInt", new BsonDocument("$convert", new BsonDocument {
                            { "input", "$startYear" },
                            { "to", "int" },
                            { "onError", BsonNull.Value },
                            { "onNull", BsonNull.Value }
                        })},
                        { "runtimeMinutesInt", new BsonDocument("$convert", new BsonDocument {
                            { "input", "$runtimeMinutes" },
                            { "to", "int" },
                            { "onError", BsonNull.Value },
                            { "onNull", BsonNull.Value }
                        })}
                    }),

                    new BsonDocument("$match", new BsonDocument
                    {
                        { "startYearInt", 2010 },
                        { "genres", new BsonDocument { { "$regex", "Drama" }, { "$options", "i" } } },
                        { "runtimeMinutesInt", new BsonDocument {
                            { "$gt", 100 },
                            { "$lte", 120 }
                        }}
                    }),

                    new BsonDocument("$sort", new BsonDocument("primaryTitle", 1)),

                    new BsonDocument("$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "primaryTitle", 1 },
                        { "startYear", "$startYearInt" },
                        { "genres", 1 },
                        { "runtimeMinutes", "$runtimeMinutesInt" }
                    }),

                    new BsonDocument("$limit", 4)
                };

            var bsonResults = await _titles.Aggregate<BsonDocument>(pipelineWithLimit).ToListAsync();

            var mappedResults = bsonResults.Select(doc => new TitleDto
            {
                PrimaryTitle = doc.GetValue("primaryTitle", "").AsString,
                StartYear = doc.Contains("startYear") ? (int?)doc["startYear"].AsInt32 : null,
                Genres = doc.GetValue("genres", "").AsString,
                RuntimeMinutes = doc.Contains("runtimeMinutes") ? (int?)doc["runtimeMinutes"].AsInt32 : null
            }).ToList();

            var pipelineForCount = new[]
            {
                new BsonDocument("$addFields", new BsonDocument
                {
                    { "startYearInt", new BsonDocument("$convert", new BsonDocument {
                        { "input", "$startYear" },
                        { "to", "int" },
                        { "onError", BsonNull.Value },
                        { "onNull", BsonNull.Value }
                    })},
                    { "runtimeMinutesInt", new BsonDocument("$convert", new BsonDocument {
                        { "input", "$runtimeMinutes" },
                        { "to", "int" },
                        { "onError", BsonNull.Value },
                        { "onNull", BsonNull.Value }
                    })}
                }),

                new BsonDocument("$match", new BsonDocument
                {
                    { "startYearInt", 2010 },
                    { "genres", new BsonDocument { { "$regex", "Drama" }, { "$options", "i" } } },
                    { "runtimeMinutesInt", new BsonDocument {
                        { "$gt", 100 },
                        { "$lte", 120 }
                    }}
                }),

                new BsonDocument("$count", "total")
            };

            var countDoc = await _titles.Aggregate<BsonDocument>(pipelineForCount).FirstOrDefaultAsync();
            var totalCount = countDoc?["total"].AsInt32 ?? 0;

            return (mappedResults, totalCount);
        }

        public async Task<List<TitleTypeCountDto>> GetTitleTypeCountsFor2020Async()
        {

            var pipeline = new[]
            {
                new BsonDocument("$addFields", new BsonDocument("startYearInt",
                    new BsonDocument("$convert", new BsonDocument
                    {
                        { "input", "$startYear" },
                        { "to", "int" },
                        { "onError", BsonNull.Value },
                        { "onNull", BsonNull.Value }
                    }))
                ),

                new BsonDocument("$match", new BsonDocument("startYearInt", 2020)),

                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$titleType" },
                    { "count", new BsonDocument("$sum", 1) }
                }),

                new BsonDocument("$sort", new BsonDocument("count", -1))
            };

            var results = await _titles.Aggregate<BsonDocument>(pipeline).ToListAsync();

            return results.Select(doc => new TitleTypeCountDto
            {
                TitleType = doc["_id"].AsString,
                Count = doc["count"].AsInt32
            }).ToList();
        }

        public async Task<ApproachResult> GetTopRatedDocumentariesAggregateAsync()
        {
            var sw = Stopwatch.StartNew();

            var pipeline = new[]
            {
                new BsonDocument("$addFields", new BsonDocument("startYearInt",
                    new BsonDocument("$convert", new BsonDocument {
                        { "input", "$startYear" },
                        { "to", "int" },
                        { "onError", BsonNull.Value },
                        { "onNull", BsonNull.Value }
                    }))),

                new BsonDocument("$match", new BsonDocument {
                    { "startYearInt", new BsonDocument { { "$gte", 2015 }, { "$lte", 2018 } } },
                    { "genres", new BsonDocument { { "$regex", "Documentary" }, { "$options", "i" } } }
                }),

                new BsonDocument("$lookup", new BsonDocument {
                    { "from", "Rating" },
                    { "localField", "tconst" },
                    { "foreignField", "tconst" },
                    { "as", "rating" }
                }),

                new BsonDocument("$unwind", "$rating"),

                new BsonDocument("$project", new BsonDocument {
                    { "primaryTitle", 1 },
                    { "startYear", "$startYearInt" },
                    { "averageRating", "$rating.averageRating" }
                }),

                new BsonDocument("$sort", new BsonDocument("averageRating", -1)),
                new BsonDocument("$limit", 3)
            };

            var docs = await _titles.Aggregate<BsonDocument>(pipeline).ToListAsync();

            var mapped = docs.Select(d => new DocumentaryRatingDto
            {
                PrimaryTitle = d.GetValue("primaryTitle", "").AsString,
                StartYear = d.GetValue("startYear", BsonNull.Value).IsBsonNull ? null : (int?)d["startYear"].AsInt32,
                AverageRating = d.GetValue("averageRating", BsonNull.Value).IsBsonNull ? null : (double?)d["averageRating"].AsDouble
            }).ToList();

            var countPipeline = pipeline.Take(6).ToList();
            countPipeline.Add(new BsonDocument("$count", "total"));
            var countDoc = await _titles.Aggregate<BsonDocument>(countPipeline).FirstOrDefaultAsync();
            var total = countDoc?["total"].AsInt32 ?? 0;

            sw.Stop();
            return new ApproachResult
            {
                Approach = "Aggregate",
                Top3 = mapped,
                TotalCount = total,
                TimeMs = sw.ElapsedMilliseconds
            };
        }

        public async Task<ApproachResult> GetTopRatedDocumentariesAggregateIndexAsync()
        {
            

            var baseResult = await GetTopRatedDocumentariesAggregateAsync();
            var result = new ApproachResult
            {
                Approach = "Aggregate + Index",
                Top3 = baseResult.Top3,
                TotalCount = baseResult.TotalCount,
                TimeMs = baseResult.TimeMs
            };

            return result;
        }


        public async Task<ApproachResult> GetTopRatedDocumentariesFindAsync()
        {
            var sw = Stopwatch.StartNew();

            var allTitles = await _titles.Find(_ => true).ToListAsync();
            var filtered = allTitles
                .Where(t =>
                    t.StartYear != null &&
                    int.TryParse(t.StartYear.ToString(), out int year) &&
                    year >= 2015 && year <= 2018 &&
                    !string.IsNullOrEmpty(t.Genres) &&
                    t.Genres.Contains("Documentary"))
                .Select(t => new
                {
                    t.TConst,
                    t.PrimaryTitle,
                    StartYear = int.Parse(t.StartYear.ToString())
                })
                .ToList();


            var tconsts = filtered.Select(f => f.TConst).ToList();

            var ratingDocs = await _ratings.Find(r => tconsts.Contains(r.TConst)).ToListAsync();
            var ratingsDict = ratingDocs.ToDictionary(r => r.TConst, r => r.AverageRating);

            var joined = filtered
                .Where(f => ratingsDict.ContainsKey(f.TConst))
                .Select(f => new DocumentaryRatingDto
                {
                    PrimaryTitle = f.PrimaryTitle.ToString(),
                    StartYear = f.StartYear,
                    AverageRating = ratingsDict[f.TConst]
                })
                .OrderByDescending(r => r.AverageRating)
                .Take(3)
                .ToList();

            var total = filtered.Count(f => ratingsDict.ContainsKey(f.TConst));

            sw.Stop();
            return new ApproachResult
            {
                Approach = "Find",
                Top3 = joined,
                TotalCount = total,
                TimeMs = sw.ElapsedMilliseconds
            };
        }

        public async Task<UpdateResultDto> SetMaxUsingAggregateAsync()
        {
            var sw = Stopwatch.StartNew();

            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("averageRating", 10.0)),
                new BsonDocument("$project", new BsonDocument("tconst", 1))
            };

            var topRated = await _ratings.Aggregate<BsonDocument>(pipeline).ToListAsync();
            var tconsts = topRated.Select(d => d["tconst"].AsString).ToList();

            var updateFilter = Builders<Title>.Filter.In(t => t.TConst, tconsts);
            var update = Builders<Title>.Update.Set("max", 1);

            var result = await _titles.UpdateManyAsync(updateFilter, update);

            sw.Stop();
            return new UpdateResultDto
            {
                ModifiedCount = (int)result.ModifiedCount,
                TimeMs = sw.ElapsedMilliseconds
            };
        }


        public async Task<UpdateResultDto> SetMaxUsingAggregateWithIndexAsync()
        {
            await _ratings.Indexes.CreateOneAsync(
                new CreateIndexModel<Rating>(Builders<Rating>.IndexKeys.Ascending(r => r.AverageRating)));

            await _titles.Indexes.CreateOneAsync(
                new CreateIndexModel<Title>(Builders<Title>.IndexKeys.Ascending(t => t.TConst)));

            return await SetMaxUsingAggregateAsync();
        }

        public async Task<UpdateResultDto> SetMaxUsingFindAsync()
        {
            var sw = Stopwatch.StartNew();

            var ratingFilter = Builders<Rating>.Filter.Eq(r => r.AverageRating, 10.0);
            var topRated = await _ratings.Find(ratingFilter).Project(r => r.TConst).ToListAsync();

            var updateFilter = Builders<Title>.Filter.In(t => t.TConst, topRated);
            var update = Builders<Title>.Update.Set("max", 1);

            var result = await _titles.UpdateManyAsync(updateFilter, update);

            sw.Stop();
            return new UpdateResultDto
            {
                ModifiedCount = (int)result.ModifiedCount,
                TimeMs = sw.ElapsedMilliseconds
            };
        }

    }
}
