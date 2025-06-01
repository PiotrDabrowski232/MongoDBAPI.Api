using MongoDBAPI.Data.Models;
using MongoDBAPI.Data.Models.Name;

namespace MongoDBAPI.Data.Services.Interfaces
{
    public interface INameService
    {
        Task<List<NameObj>> GetNames();
        Task<List<CountResult>> NameCount(CancellationToken ct);
        public Task<(int TotalCount, List<Models.Name.Name.NameDto> Top5)> FindPeopleBySurnameAsync();
        public Task<IndexListResult> CreateBirthYearDescendingIndexAndListAsync();

    }
}
