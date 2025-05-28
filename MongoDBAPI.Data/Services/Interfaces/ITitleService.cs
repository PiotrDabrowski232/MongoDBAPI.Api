using MongoDB.Driver;
using MongoDBAPI.Data.Models;

namespace MongoDBAPI.Data.Services.Interfaces
{
    public interface ITitleService
    {
        Task<List<Title>> GetTitles();
    }
}
