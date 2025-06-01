using MongoDB.Bson;

namespace MongoDBAPI.Data.Models
{
    public class IndexListResult
    {
        public int IndexCount { get; set; }
        public List<Dictionary<string, object>> Indexes { get; set; }
    }
}
