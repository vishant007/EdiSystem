namespace EDI315Payment.Models
{
    public class PaymentSqlTransaction
    {
        public int Id { get; set; } 
        public string TransactionId { get; set; } 
        public string UserId { get; set; }
        public decimal TotalDemurrageFees { get; set; }
        public decimal OtherPayments { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
