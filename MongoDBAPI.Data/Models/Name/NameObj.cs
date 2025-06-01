using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MongoDBAPI.Data.Models.Name
{
    public class NameObj
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("nconst")]
        public string Nconst { get; set; }

        [BsonElement("primaryName")]
        public string PrimaryName { get; set; }

        [BsonElement("birthYear")]
        public BsonValue BirthYear { get; set; }

        [BsonElement("deathYear")]
        public BsonValue DeathYear { get; set; }

        [BsonElement("primaryProfession")]
        public string PrimaryProfession { get; set; }

        [BsonElement("knownForTitles")]
        public string KnownForTitles { get; set; }
    }
}
