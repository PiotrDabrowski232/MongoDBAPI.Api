using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MongoDBAPI.Data.Models
{
    public class Name
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("nconst")]
        public string Nconst { get; set; }

        [BsonElement("primaryName")]
        public string PrimaryName { get; set; }

        [BsonElement("birthYear")]
        public string BirthYear { get; set; }

        [BsonElement("deathYear")]
        public string DeathYear { get; set; }

        [BsonElement("primaryProfession")]
        public string PrimaryProfession { get; set; }

        [BsonElement("knownForTitles")]
        public string KnownForTitles { get; set; }
    }
}
