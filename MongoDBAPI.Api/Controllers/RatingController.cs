using Microsoft.AspNetCore.Mvc;
using MongoDBAPI.Data.Models;
using MongoDBAPI.Data.Models.Rating;
using MongoDBAPI.Data.Services;
using MongoDBAPI.Data.Services.Interfaces;

namespace MongoDBAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet]
        [Route("GetRatings")]
        public async Task<ActionResult<List<Rating>>> GetAll()
        {
            var ratings = await _ratingService.GetRatings();
            return Ok(ratings);
        }

        [HttpGet]
        [Route("GetCount")]
        public async Task<ActionResult<List<CountResult>>> GetCount( CancellationToken ct)
        {
            var result = await _ratingService.RatingCount(ct);
            return Ok(result);
        }
    }
}
