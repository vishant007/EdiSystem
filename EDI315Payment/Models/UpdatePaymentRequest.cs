namespace EDI315Payment.Models
{
    public class UpdatePaymentRequest
    {
        public string ContainerNumber { get; set; }
        public decimal TotalDemurrageFees { get; set; }
        public decimal OtherPayments { get; set; }
    }
}
