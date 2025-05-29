using Microsoft.AspNetCore.Mvc;
using MongoDBAPI.Data.Models;
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
        public async Task<ActionResult<List<Name>>> GetAll()
        {
            var names = await _nameService.GetNames();
            return Ok(names);
        }
    }
}
