using Microsoft.AspNetCore.Mvc;
using MongoDBAPI.Data.Models;
using MongoDBAPI.Data.Models.Name;
using MongoDBAPI.Data.Models.Name.Name;
using MongoDBAPI.Data.Services.Interfaces;

namespace MongoDBAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NameController : ControllerBase
    {
        private readonly INameService _nameService;

        public NameController(INameService nameService)
        {
            _nameService = nameService;
        }

        [HttpGet]
        [Route("GetNames")]
        public async Task<ActionResult<List<NameObj>>> GetAll()
        {
            var names = await _nameService.GetNames();
            return Ok(names);
        }

        [HttpGet]
        [Route("GetCount")]
        public async Task<ActionResult<List<CountResult>>> GetCount(CancellationToken ct)
        {
            var result = await _nameService.NameCount(ct);
            return Ok(result);
        }

        [HttpGet]
        [Route("FindPeopleByNameAsync")]
        public async Task<IActionResult> FindPeopleByNameAsync()
        {
            (int count, List<NameDto> people) = await _nameService.FindPeopleBySurnameAsync();
            return Ok(new
            {
                totalCount = count,
                top5 = people
            });
        }

        [HttpGet]
        [Route("CreateBirthYearDescendingIndexAndListAsync")]
        public async Task<ActionResult<IndexListResult>> CreateBirthYearDescendingIndexAndListAsync()
        {
            var result = await _nameService.CreateBirthYearDescendingIndexAndListAsync();
            return Ok(result);
        }
    }
}
