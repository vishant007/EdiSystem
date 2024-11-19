namespace EDI315Parser.Models
{
    public class B4
    {
        public string SpecialHandlingCode { get; set; }
        public string ShipmentStatusCode { get; set; }
        public string EquipmentStatusCode { get; set; }
        public string EquipmentType { get; set; }
        public string ContainerNumber { get; set; }
        public DateOnly Date { get; set; }
    }
}
