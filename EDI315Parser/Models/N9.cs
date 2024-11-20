namespace EDI315Parser.Models;
public class N9
{
    public string ReferenceCode { get; set; }
    public string FeeType { get; set; }
    public decimal FeeAmount { get; set; }
    public DateTime? FeeUntilDate { get; set; } 
    public TimeSpan? FeeUntilTime { get; set; } 
     public decimal TotalDemurrageFees { get; set; } = 0; 
    public decimal OtherPayments { get; set; } = 0;
}
