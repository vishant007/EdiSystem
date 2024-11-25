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
        private readonly AzureServiceBusService _serviceBusService;



        public WatchlistController(IWatchlistRepository watchlistRepository, CosmosDbService cosmosDbService, AzureServiceBusService serviceBusService)
        {
            _watchlistRepository = watchlistRepository;
            _cosmosDbService = cosmosDbService;
            _serviceBusService = serviceBusService;

        }

        [HttpPost("{userId}/add")]
        public async Task<IActionResult> AddToWatchlist(string userId, [FromBody] string containerNumber)
        {
            // Add container to watchlist
            var watchlistItem = new WatchlistModel
            {
                UserId = userId,
                ContainerNumber = containerNumber
            };
            await _watchlistRepository.AddToWatchlistAsync(watchlistItem);

            // Fetch additional details from Cosmos DB
            var containerDetails = await _cosmosDbService.GetContainerDetailsAsync(containerNumber);
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

            // Send message to Azure Service Bus
            await _serviceBusService.SendMessageAsync(message);

            return Ok("Container added to watchlist and message sent to Service Bus.");
        }

        [HttpPost("{userId}/remove")]
        public async Task<IActionResult> RemoveFromWatchlist(string userId, [FromBody] string containerNumber)
        {
            // Remove container from the watchlist
            var isRemoved = await _watchlistRepository.RemoveFromWatchlistAsync(userId, containerNumber);
            
            if (!isRemoved)
            {
                return NotFound("Container not found in watchlist.");
            }

            // Fetch additional details from Cosmos DB to notify if necessary
            var containerDetails = await _cosmosDbService.GetContainerDetailsAsync(containerNumber);
            var message = new
            {
                UserId = userId,
                ContainerNumber = containerNumber,
                RemovalStatus = "Container removed from watchlist",
                ContainerDetails = containerDetails
            };

            // Send removal message to Azure Service Bus
            await _serviceBusService.SendMessageAsync(message);

            return Ok("Container removed from watchlist and removal message sent to Service Bus.");
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
