using MongoDBAPI.Data.Models;
using MongoDBAPI.Data.Models.Rating;
using MongoDBAPI.Data.Models.Title;

namespace MongoDBAPI.Data.Services.Interfaces
{
    public interface ITitleService
    {
        Task<List<Title>> GetTitles();
        public Task<List<CountResult>> TitleCount(CancellationToken ct);

        public Task<(List<TitleDto> Results, long TotalCount)> GetFilteredTitlesAsync(CancellationToken ct);
        public Task<List<TitleTypeCountDto>> GetTitleTypeCountsFor2020Async();

        //zadanie 4
        public  Task<ApproachResult> GetTopRatedDocumentariesAggregateAsync();
        public Task<ApproachResult> GetTopRatedDocumentariesAggregateIndexAsync();
        public Task<ApproachResult> GetTopRatedDocumentariesFindAsync();

        //Zadanie 7
        public Task<UpdateResultDto> SetMaxUsingAggregateAsync();
        public Task<UpdateResultDto> SetMaxUsingAggregateWithIndexAsync();
        public Task<UpdateResultDto> SetMaxUsingFindAsync();

        //Zadanie 8
        public Task<TitleAverageRatingDto?> GetAverageRatingForTitleAsync(string title, int year, CancellationToken ct);

        //Zadanie 9
        Task<UpdateResultDto> AddRatingArrayToBladeRunnerAsync(CancellationToken ct);

        //Zadanie10
        Task<UpdateResultDto> AddCustomRatingToBladeRunnerAsync(CancellationToken ct);

        // Zadanie 11
        Task<UpdateResultDto> RemoveRatingFromBladeRunnerAsync(CancellationToken ct);

        // Zadanie 12
        Task<UpdateResultDto> UpsertAvgRatingForPanTadeuszAsync(CancellationToken ct);

        // Zadanie 13
        Task<UpdateResultDto> DeleteTitlesBefore1964Async(CancellationToken ct);


    }
}
