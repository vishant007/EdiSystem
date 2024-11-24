namespace EDI315Payment.Models
{
    public class MsgData
    {
        public string id { get; set; }
        public string PartitionKey { get; set; }
        public string ContainerNumber { get; set; }
        public string FeeStatus { get; set; }
        public decimal TotalDemurrageFees { get; set; }
        public decimal OtherPayments { get; set; }
        public string UserId { get; set; }
    }
}
