namespace EDI315Payment.Models
{
    public class PaymentSqlTransaction
    {
        public int Id { get; set; } // Primary Key
        public string TransactionId { get; set; } // GUID for the transaction
        public string UserId { get; set; }
        public decimal TotalDemurrageFees { get; set; }
        public decimal OtherPayments { get; set; }
        public DateTime TransactionDate { get; set; } // Optional: Track transaction time
    }
}
