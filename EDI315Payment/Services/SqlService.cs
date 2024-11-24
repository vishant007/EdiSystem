using EDI315Payment.Data;
using EDI315Payment.Models;
using System.Threading.Tasks;

namespace EDI315Payment.Services
{
    public class SqlService
    {
        private readonly PaymentDbContext _dbContext;

        public SqlService(PaymentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Method to save a payment transaction to SQL
        public async Task SaveTransactionAsync(PaymentSqlTransaction transaction)
        {
            _dbContext.PaymentSqlTransactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
        }
    }
}
