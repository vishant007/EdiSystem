using System.Collections.Generic;
using System.Threading.Tasks;
using EDI315Api.Models;
using EDI315Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EDI315Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchlistController : ControllerBase
    {
        private readonly IWatchlistRepository _watchlistRepository;

        public WatchlistController(IWatchlistRepository watchlistRepository)
        {
            _watchlistRepository = watchlistRepository;
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
    }
}
