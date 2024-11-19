using Microsoft.EntityFrameworkCore;
using EDI315Api.Models;

namespace EDI315Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<WatchlistModel> Watchlist { get; set; }
    }
}
