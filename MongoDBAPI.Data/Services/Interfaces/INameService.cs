using MongoDBAPI.Data.Models;

namespace MongoDBAPI.Data.Services.Interfaces
{
    public interface INameService
    {
        Task<List<Name>> GetNames();
    }
}
