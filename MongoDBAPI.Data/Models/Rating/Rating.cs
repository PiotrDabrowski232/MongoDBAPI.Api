using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBAPI.Data.Models.Rating
{
    public class Rating
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("tconst")]
        public string TConst { get; set; }

        [BsonElement("averageRating")]
        public double AverageRating { get; set; }

        [BsonElement("numVotes")]
        public int NumVotes { get; set; }
    }
}
