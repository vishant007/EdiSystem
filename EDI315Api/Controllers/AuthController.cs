using Microsoft.AspNetCore.Mvc;
using System;
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
            try
            {
                await _userService.Register(user);
                return Ok(new { Message = "User registered successfully!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User user)
        {
            try
            {
                var token = await _userService.Authenticate(user);
                if (token == null)
                {
                    return Unauthorized(new { Message = "Invalid email or password!" });
                }

                return Ok(new
                {
                    Token = token,
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }
    }
}
