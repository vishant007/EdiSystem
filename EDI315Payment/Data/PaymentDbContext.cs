using EDI315Payment.Models;
using Microsoft.EntityFrameworkCore;

namespace EDI315Payment.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
            : base(options)
        {
        }

        public DbSet<PaymentSqlTransaction> PaymentSqlTransactions { get; set; }
    }
}
