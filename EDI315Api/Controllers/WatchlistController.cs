using System.Collections.Generic;
using System.Threading.Tasks;
using EDI315Api.Models;
using EDI315Api.Repositories;
using EDI315Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EDI315Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WatchlistController : ControllerBase
    {
        private readonly IWatchlistRepository _watchlistRepository;
        private readonly CosmosDbService _cosmosDbService;
        private readonly AzureServiceBusService _serviceBusService;

        public WatchlistController(
            IWatchlistRepository watchlistRepository,
            CosmosDbService cosmosDbService,
            AzureServiceBusService serviceBusService)
        {
            _watchlistRepository = watchlistRepository;
            _cosmosDbService = cosmosDbService;
            _serviceBusService = serviceBusService;
        }

        [HttpPost("{userId}/add")]
        public async Task<IActionResult> AddToWatchlist(string userId, [FromBody] string containerNumber)
        {

            // Check if container is already in the user's watchlist
            var existingWatchlist = await _watchlistRepository.GetUserWatchlistAsync(userId);
            if (existingWatchlist != null && existingWatchlist.Exists(w => w.ContainerNumber == containerNumber))
            {
                return Conflict($"Container '{containerNumber}' is already in the watchlist for user ID '{userId}'.");
            }

            // Add container to watchlist
            var watchlistItem = new WatchlistModel
            {
                UserId = userId,
                ContainerNumber = containerNumber
            };
            await _watchlistRepository.AddToWatchlistAsync(watchlistItem);

            // Fetch additional details from Cosmos DB
            var containerDetails = await _cosmosDbService.GetContainerDetailsAsync(containerNumber);
            if (containerDetails == null)
            {
                return NotFound($"Details for container number {containerNumber} not found in Cosmos DB.");
            }

            var message = new
            {
                UserId = userId,
                ContainerNumber = containerNumber,
                TotalDemurrageFees = containerDetails?["TotalDemurrageFees"],
                OtherPayments = containerDetails?["OtherPayments"],
                FeeStatus = containerDetails?["FeeStatus"],
                Id = containerDetails?["id"],
                PartitionKey = containerDetails?["PartitionKey"]
            };

            await _serviceBusService.SendMessageAsync(message);

            return Ok("Container added to watchlist and message sent to Service Bus.");
        }

        [HttpPost("{userId}/remove")]
        public async Task<IActionResult> RemoveFromWatchlist(string userId, [FromBody] string containerNumber)
        {

            
            var isRemoved = await _watchlistRepository.RemoveFromWatchlistAsync(userId, containerNumber);
            if (!isRemoved)
            {
                return NotFound("Container not found in watchlist.");
            }

            return Ok("Container removed from watchlist.");
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<WatchlistModel>>> GetUserWatchlist(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var watchlist = await _watchlistRepository.GetUserWatchlistAsync(userId);
            return Ok(watchlist);
        }

        [HttpGet("details/{containerNumber}")]
        public async Task<IActionResult> GetContainerDetails(string containerNumber)
        {
            if (string.IsNullOrWhiteSpace(containerNumber))
            {
                return BadRequest("Container number cannot be null or empty.");
            }

            var containerDetails = await _cosmosDbService.GetContainerDetailsAsync(containerNumber);
            if (containerDetails == null)
            {
                return NotFound("Container details not found.");
            }

            return Ok(containerDetails);
        }
    }
}
