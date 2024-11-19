using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EDI315Api.Models;
using EDI315Api.Services;

namespace EDI315Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly CosmosDbService _cosmosDbService;

        public AuthController(IUserService userService, CosmosDbService cosmosDbService)
        {
            _userService = userService;
            _cosmosDbService = cosmosDbService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            await _userService.Register(user);
            return Ok(new { Message = "User registered successfully!" });
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User user)
        {
            var token = await _userService.Authenticate(user);
            if (token == null)
            {
                return Unauthorized(new { Message = "Invalid email or password!" });
            }

            // Fetch data from Cosmos DB upon successful login
            var cosmosData = await _cosmosDbService.GetAllItemsAsync();

            return Ok(new
            {
                Token = token,
                CosmosData = cosmosData
            });
        }

    }
}
