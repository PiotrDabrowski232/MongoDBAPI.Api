using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBAPI.Data.Models.Title
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
        public BsonValue PrimaryTitle { get; set; }

        [BsonElement("originalTitle")]
        public BsonValue OriginalTitle { get; set; }

        [BsonElement("isAdult")]
        public int IsAdult { get; set; }  // Dane są Int32 w Mongo

        [BsonElement("startYear")]
        public BsonValue StartYear { get; set; }

        [BsonElement("endYear")]
        public BsonValue EndYear { get; set; }

        [BsonElement("runtimeMinutes")]
        public BsonValue RuntimeMinutes { get; set; }

        [BsonElement("genres")]
        public string Genres { get; set; }
    }
}
