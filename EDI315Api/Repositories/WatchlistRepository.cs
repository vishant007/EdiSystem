using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDI315Api.Models;
using Microsoft.EntityFrameworkCore;
using EDI315Api.Data;

namespace EDI315Api.Repositories
{
    public class WatchlistRepository : IWatchlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WatchlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddToWatchlistAsync(WatchlistModel watchlistItem)
        {
            _context.Watchlist.Add(watchlistItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WatchlistModel>> GetUserWatchlistAsync(string userId)
        {
            return await _context.Watchlist
                .Where(w => w.UserId == userId)
                .ToListAsync();
        }
    }
}
