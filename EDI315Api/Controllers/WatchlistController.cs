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

     
        public WatchlistController(IWatchlistRepository watchlistRepository, CosmosDbService cosmosDbService)
        {
            _watchlistRepository = watchlistRepository;
            _cosmosDbService = cosmosDbService;
        }

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

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<WatchlistModel>>> GetUserWatchlist(string userId)
        {
            var watchlist = await _watchlistRepository.GetUserWatchlistAsync(userId);
            return Ok(watchlist);
        }
        
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
