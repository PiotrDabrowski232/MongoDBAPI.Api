using MongoDBAPI.Data.Models;

namespace MongoDBAPI.Data.Services.Interfaces
{
    public interface IRatingService
    {
        Task<List<Rating>> GetRatings();
    }
}
