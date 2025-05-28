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
        [BsonSerializer(typeof(FlexibleStringDeserializer))]
        public string TConst { get; set; }

        [BsonElement("titleType")]
        [BsonSerializer(typeof(FlexibleStringDeserializer))]
        public string TitleType { get; set; }

        [BsonElement("primaryTitle")]
        [BsonSerializer(typeof(FlexibleStringDeserializer))]
        public string PrimaryTitle { get; set; }

        [BsonElement("originalTitle")]
        [BsonSerializer(typeof(FlexibleStringDeserializer))]
        public string OriginalTitle { get; set; }

        [BsonElement("isAdult")]
        [BsonSerializer(typeof(FlexibleBooleanDeserializer))]
        public bool IsAdult { get; set; }

        [BsonElement("startYear")]
        [BsonSerializer(typeof(NullableInt32OrStringDeserializer))]
        public int? StartYear { get; set; }

        [BsonElement("endYear")]
        [BsonSerializer(typeof(FlexibleStringDeserializer))]
        public string EndYear { get; set; }

        [BsonElement("runtimeMinutes")]
        [BsonSerializer(typeof(NullableInt32OrStringDeserializer))]
        public int? RuntimeMinutes { get; set; }

        [BsonElement("genres")]
        [BsonSerializer(typeof(FlexibleStringDeserializer))]
        public string Genres { get; set; }
    }
}
