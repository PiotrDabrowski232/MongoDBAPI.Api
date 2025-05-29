using Microsoft.AspNetCore.Mvc;
using MongoDBAPI.Data.Models;
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
    }
}
