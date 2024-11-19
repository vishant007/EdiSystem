namespace EDI315Parser.Models
{
    public class ISA
    {
        public string Authorization_Information_Qualifier { get; set; }
        public string Secutity_Information_Qualifier { get; set; }
        public string Interchange_Id_Qualifier_Sender { get; set; }
        public string Interchange_Sender_Id_Qualifier { get; set; }
        public string Interchange_Id_Qualifier { get; set; }
        public string Interchange_Receiver_Id { get; set; }
        public string Interchange_Control_Standard_Id_Code { get; set; }
        public string Interchange_Version { get; set; }
        public string Interchange_Control_Number { get; set; }
        public bool? Acknowledgement_Requested { get; set; }  
        public bool Test_Indicator { get; set; } 
        public string Sub_Element_Separator { get; set; }
        public DateTime? DateTime { get; set; } 
    }
}
