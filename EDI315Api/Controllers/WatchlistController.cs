using System.Collections.Generic;
using System.Threading.Tasks;
using EDI315Api.Models;
using EDI315Api.Repositories;
using EDI315Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EDI315Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchlistController : ControllerBase
    {
        private readonly IWatchlistRepository _watchlistRepository;
        private readonly CosmosDbService _cosmosDbService;

        // Constructor to inject dependencies
        public WatchlistController(IWatchlistRepository watchlistRepository, CosmosDbService cosmosDbService)
        {
            _watchlistRepository = watchlistRepository;
            _cosmosDbService = cosmosDbService;
        }

        // Endpoint to add a container to a user's watchlist
        [HttpPost("{userId}/add")]
        public async Task<IActionResult> AddToWatchlist(string userId, [FromBody] string containerNumber)
        {
            var watchlistItem = new WatchlistModel
            {
                UserId = userId,
                ContainerNumber = containerNumber
            };

            await _watchlistRepository.AddToWatchlistAsync(watchlistItem);
            return Ok("Container added to watchlist.");
        }

        // Endpoint to get a user's entire watchlist
        [HttpGet("{userId}")]
        public async Task<ActionResult<List<WatchlistModel>>> GetUserWatchlist(string userId)
        {
            var watchlist = await _watchlistRepository.GetUserWatchlistAsync(userId);
            return Ok(watchlist);
        }

        // Endpoint to get detailed information about a container
        [HttpGet("details/{containerNumber}")]
        public async Task<IActionResult> GetContainerDetails(string containerNumber)
        {
            var containerDetails = await _cosmosDbService.GetContainerDetailsAsync(containerNumber);
            if (containerDetails == null)
            {
                return NotFound("Container details not found.");
            }

            return Ok(containerDetails);
        }
    }
}
