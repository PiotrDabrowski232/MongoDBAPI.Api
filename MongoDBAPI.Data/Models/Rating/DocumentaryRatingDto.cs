namespace MongoDBAPI.Data.Models.Rating
{
    public class DocumentaryRatingDto
    {
        public string PrimaryTitle { get; set; }
        public int? StartYear { get; set; }
        public double? AverageRating { get; set; }
    }
}
