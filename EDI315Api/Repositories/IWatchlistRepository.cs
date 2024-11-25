using System.Collections.Generic;
using System.Threading.Tasks;
using EDI315Api.Models;

namespace EDI315Api.Repositories
{
    public interface IWatchlistRepository
    {
        Task AddToWatchlistAsync(WatchlistModel watchlistItem);
        Task<List<WatchlistModel>> GetUserWatchlistAsync(string userId);
        Task<bool> RemoveFromWatchlistAsync(string userId, string containerNumber);
        
    }
}
