using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;

namespace MongoDBAPI.Data.Extensions
{
    public class NullableInt32OrStringDeserializer : IBsonSerializer<int?>
    {
        public Type ValueType => typeof(int?);

        public int? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.GetCurrentBsonType();

            switch (bsonType)
            {
                case BsonType.Int32:
                    return context.Reader.ReadInt32();
                case BsonType.Int64:
                    return Convert.ToInt32(context.Reader.ReadInt64());
                case BsonType.String:
                    return TryParseNullableInt(context.Reader.ReadString());
                case BsonType.Null:
                    context.Reader.ReadNull();
                    return null;
                default:
                    throw new FormatException($"Cannot deserialize BsonType {bsonType} to int?");
            }
        }

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            => Deserialize(context, args);

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, int? value)
        {
            if (value.HasValue)
                context.Writer.WriteInt32(value.Value);
            else
                context.Writer.WriteNull();
        }

        void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
            => Serialize(context, args, value as int?);

        private int? TryParseNullableInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "\\N")
                return null;

            if (int.TryParse(value, out int result))
                return result;

            return null; // lub: throw new FormatException($"Cannot parse '{value}' as int");
        }
    }
}
