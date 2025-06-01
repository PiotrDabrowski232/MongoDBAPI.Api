using MongoDBAPI.Data.Models.Rating;

namespace MongoDBAPI.Data.Models
{
    public class ApproachResult
    {
        public string Approach { get; set; }
        public List<DocumentaryRatingDto> Top3 { get; set; }
        public int TotalCount { get; set; }
        public long TimeMs { get; set; }
    }
}
