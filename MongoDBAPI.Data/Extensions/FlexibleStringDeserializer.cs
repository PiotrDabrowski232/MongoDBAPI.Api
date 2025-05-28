using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Globalization;

namespace MongoDBAPI.Data.Extensions
{
    public class FlexibleStringDeserializer : IBsonSerializer<string>
    {
        public Type ValueType => typeof(string);

        public string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.GetCurrentBsonType();

            switch (bsonType)
            {
                case BsonType.String:
                    return context.Reader.ReadString();
                case BsonType.Int32:
                    return context.Reader.ReadInt32().ToString();
                case BsonType.Int64:
                    return context.Reader.ReadInt64().ToString();
                case BsonType.Double:
                    return context.Reader.ReadDouble().ToString(CultureInfo.InvariantCulture);
                case BsonType.Boolean:
                    return context.Reader.ReadBoolean().ToString().ToLower();
                case BsonType.DateTime:
                    return BsonUtils.ToDateTimeFromMillisecondsSinceEpoch(context.Reader.ReadDateTime())
                        .ToString("o", CultureInfo.InvariantCulture); // ISO 8601 format
                case BsonType.Null:
                    context.Reader.ReadNull();
                    return null;
                default:
                    throw new FormatException($"Unsupported BsonType {bsonType} for string deserialization");
            }
        }

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            => Deserialize(context, args);

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            if (value == null)
                context.Writer.WriteNull();
            else
                context.Writer.WriteString(value);
        }

        void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
            => Serialize(context, args, value as string);
    }
}
