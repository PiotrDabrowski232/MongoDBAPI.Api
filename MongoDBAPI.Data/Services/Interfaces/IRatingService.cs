using MongoDBAPI.Data.Models;
using MongoDBAPI.Data.Models.Rating;

namespace MongoDBAPI.Data.Services.Interfaces
{
    public interface IRatingService
    {
        Task<List<Rating>> GetRatings();
        Task<List<CountResult>> RatingCount(CancellationToken ct);
    }
}
