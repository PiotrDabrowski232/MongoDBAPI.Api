namespace MongoDBAPI.Data.Models
{
    public class CountResult
    {
        public string Method { get; set; }
        public long DocumentsCount { get; set; }
        public long Time { get; set; }
    }
}
