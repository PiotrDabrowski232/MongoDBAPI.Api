using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDBAPI.Data.Extensions;

namespace MongoDBAPI.Data.Models
{
    public class Title
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("tconst")]
        public string TConst { get; set; }

        [BsonElement("titleType")]
        public string TitleType { get; set; }

        [BsonElement("primaryTitle")]
        public string PrimaryTitle { get; set; }

        [BsonElement("originalTitle")]
        public string OriginalTitle { get; set; }

        [BsonElement("isAdult")]
        public string IsAdult { get; set; }

        [BsonElement("startYear")]
        public string StartYear { get; set; }

        [BsonElement("endYear")]
        public string EndYear { get; set; }

        [BsonElement("runtimeMinutes")]
        public string RuntimeMinutes { get; set; }

        [BsonElement("genres")]
        public string Genres { get; set; }
    }
}
