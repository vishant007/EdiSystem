public class Payment
{
    public string Id { get; set; }
    public string ContainerNumber { get; set; }
    public string FeeStatus { get; set; }
    public double TotalDemurrageFees { get; set; }
    public double OtherPayments { get; set; }
    public string PartitionKey { get; set; } 
}
