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
        public async Task<bool> RemoveFromWatchlistAsync(string userId, string containerNumber)
        {
            
            var watchlistItem = await _context.Watchlist
                .Where(w => w.UserId == userId && w.ContainerNumber == containerNumber)
                .FirstOrDefaultAsync();

            if (watchlistItem != null)
            {
                _context.Watchlist.Remove(watchlistItem);  
                await _context.SaveChangesAsync();  
                return true;  
            }

            return false;  
        }
    }
}
