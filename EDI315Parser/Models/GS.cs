namespace EDI315Parser.Models
{
    public class GS
    {
        public string Functional_Identifier_Code { get; set; }
        public string Functional_SendersCode { get; set; }
        public string Functional_Receivers_Code { get; set; }
        public string Group_Control_Number { get; set; }
        public string Responsible_Agency_Code { get; set; }
        public string Version { get; set; }
        public DateTime DateTime { get; set; }  
    }
}
