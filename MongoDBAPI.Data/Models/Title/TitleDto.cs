namespace MongoDBAPI.Data.Models.Title
{
    public class TitleDto
    {
        public string PrimaryTitle { get; set; }
        public int? StartYear { get; set; }
        public string Genres { get; set; }
        public int? RuntimeMinutes { get; set; }
    }
}
