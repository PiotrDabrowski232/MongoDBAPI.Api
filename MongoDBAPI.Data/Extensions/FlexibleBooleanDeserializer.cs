using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace MongoDBAPI.Data.Extensions
{
    public class FlexibleBooleanDeserializer : IBsonSerializer<bool>
    {
        public Type ValueType => typeof(bool);

        public bool Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.GetCurrentBsonType();

            return bsonType switch
            {
                BsonType.Boolean => context.Reader.ReadBoolean(),
                BsonType.String => TryParseBoolean(context.Reader.ReadString()),
                BsonType.Int32 => context.Reader.ReadInt32() != 0,
                BsonType.Int64 => context.Reader.ReadInt64() != 0,
                BsonType.Null => false,
                _ => throw new FormatException($"Unsupported BsonType {bsonType} for boolean deserialization")
            };
        }

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            => Deserialize(context, args);

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, bool value)
        {
            context.Writer.WriteBoolean(value);
        }

        void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
            => Serialize(context, args, (bool)value);

        private bool TryParseBoolean(string value)
        {
            if (bool.TryParse(value, out var result))
                return result;

            return false; // lub throw, jeśli chcesz surowiej obsłużyć
        }
    }
}
