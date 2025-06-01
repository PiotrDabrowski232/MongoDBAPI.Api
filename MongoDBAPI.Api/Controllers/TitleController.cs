using Microsoft.AspNetCore.Mvc;
using MongoDBAPI.Data.Models;
using MongoDBAPI.Data.Models.Title;
using MongoDBAPI.Data.Services;
using MongoDBAPI.Data.Services.Interfaces;

namespace MongoDBAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitleController : ControllerBase
    {
        private readonly ITitleService _titleService;

        public TitleController(ITitleService personService)
        {
            _titleService = personService;
        }

        [HttpGet]
        [Route("GetTitles")]
        public async Task<ActionResult<List<Title>>> GetAll()
        {
            var titles = await _titleService.GetTitles();
            return Ok(titles);
        }

        [HttpGet]
        [Route("GetCount")]
        public async Task<ActionResult<List<CountResult>>> GetCount(CancellationToken ct)
        {
            var result = await _titleService.TitleCount(ct);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetFilteredTitles")]
        public async Task<IActionResult> GetFilteredTitles(CancellationToken ct)
        {
            var (results, count) = await _titleService.GetFilteredTitlesAsync(ct);

            return Ok(new
            {
                TotalMatchingDocuments = count,
                ReturnedDocuments = results.Select(r => new {
                    r.PrimaryTitle,
                    r.StartYear,
                    r.Genres,
                    r.RuntimeMinutes
                })
            });
        }

        [HttpGet]
        [Route("GetTitleTypeCounts2020")]
        public async Task<IActionResult> GetTitleTypeCounts2020()
        {
            var results = await _titleService.GetTitleTypeCountsFor2020Async();
            return Ok(results);
        }

        [HttpGet]
        [Route("GetTopDocumentariesAgregate")]
        public async Task<IActionResult> GetTopDocumentariesAgregate()
        {
            List<ApproachResult> results = new List<ApproachResult>
            {
                await _titleService.GetTopRatedDocumentariesAggregateAsync()
            };

            return Ok(results);
        }

        [HttpGet]
        [Route("GetTopDocumentariesAgregateIndex")]
        public async Task<IActionResult> GetTopDocumentariesAgregateIndex()
        {
            List<ApproachResult> results = new List<ApproachResult>
            {
                await _titleService.GetTopRatedDocumentariesAggregateIndexAsync()
            };

            return Ok(results);
        }

        [HttpGet]
        [Route("GetTopDocumentariesFind")]
        public async Task<IActionResult> GetTopDocumentariesFind()
        {
            List<ApproachResult> results = new List<ApproachResult>
            {
                await _titleService.GetTopRatedDocumentariesFindAsync()
            };

            return Ok(results);
        }

        [HttpGet]
        [Route("SetMaxUsingAggregateAsync")]
        public async Task<IActionResult> SetMaxUsingAggregateAsync()
        {
            var results = await _titleService.SetMaxUsingAggregateAsync();
            
            return Ok(results);
        }

        [HttpGet]
        [Route("SetMaxUsingAggregateWithIndexAsync")]
        public async Task<IActionResult> SetMaxUsingAggregateWithIndexAsync()
        {

            var results = await _titleService.SetMaxUsingAggregateWithIndexAsync();

            return Ok(results);
        }

        [HttpGet]
        [Route("SetMaxUsingFindAsync")]
        public async Task<IActionResult> SetMaxUsingFindAsync()
        {

            var results = await _titleService.SetMaxUsingFindAsync();

            return Ok(results);
        }

        [HttpGet]
        [Route("GetAverageRatingForTitle")]
        public async Task<IActionResult> GetAverageRatingForTitle([FromQuery] string title, [FromQuery] int year, CancellationToken ct)
        {
            var result = await _titleService.GetAverageRatingForTitleAsync(title, year, ct);
            if (result == null)
                return NotFound($"No movie found with title '{title}' and year {year}.");
            return Ok(result);
        }

        [HttpPost]
        [Route("AddRatingArrayToBladeRunner")]
        public async Task<IActionResult> AddRatingArrayToBladeRunner(CancellationToken ct)
        {
            var result = await _titleService.AddRatingArrayToBladeRunnerAsync(ct);
            if (result.ModifiedCount == 0)
                return NotFound("Blade Runner (1982) not found or already updated.");
            return Ok(result);
        }

        [HttpPost]
        [Route("AddCustomRatingToBladeRunner")]
        public async Task<IActionResult> AddCustomRatingToBladeRunner(CancellationToken ct)
        {
            var result = await _titleService.AddCustomRatingToBladeRunnerAsync(ct);
            if (result.ModifiedCount == 0)
                return NotFound("Blade Runner (1982) not found or already updated.");
            return Ok(result);
        }

        [HttpPost]
        [Route("RemoveRatingFromBladeRunner")]
        public async Task<IActionResult> RemoveRatingFromBladeRunner(CancellationToken ct)
        {
            var result = await _titleService.RemoveRatingFromBladeRunnerAsync(ct);
            if (result.ModifiedCount == 0)
                return NotFound("Blade Runner (1982) not found or rating field not present.");
            return Ok(result);
        }

        [HttpPost]
        [Route("UpsertAvgRatingForPanTadeusz")]
        public async Task<IActionResult> UpsertAvgRatingForPanTadeusz(CancellationToken ct)
        {
            var result = await _titleService.UpsertAvgRatingForPanTadeuszAsync(ct);
            return Ok(result);
        }

        [HttpDelete]
        [Route("DeleteTitlesBefore1964")]
        public async Task<IActionResult> DeleteTitlesBefore1964(CancellationToken ct)
        {
            var result = await _titleService.DeleteTitlesBefore1964Async(ct);
            return Ok(new { DeletedCount = result.ModifiedCount, TimeMs = result.TimeMs });
        }

    }
}
