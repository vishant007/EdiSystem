// using System.Text.Json.Serialization;

// namespace EDI315Parser.Models
// {
//     public class MsgData
//     {
//         public string id { get; set; } = Guid.NewGuid().ToString();
//         public string PartitionKey { get; set; } = "EDI315Data";
//         public ISA isaSegment { get; set; }
//         public GS gsSegment { get; set; }
//         public ST stSegment { get; set; }
//         public B4 b4Segment { get; set; }
//         public N9 n9Segment { get; set; }
//         public Q2 q2Segment { get; set; }
//         public SG sgSegment { get; set; }
//         public R4 r4Segment { get; set; }
//         public SE seSegment { get; set; }
//         public GE geSegment { get; set; }
//         public IEA ieaSegment { get; set; }

//     }

//     public class ISA
//     {
//         public string Authorization_Information_Qualifier { get; set; }

//         public string Secutity_Information_Qualifier { get; set; }


//         public string Interchange_Id_Qualifier_Sender { get; set; }

//         public string Interchange_Sender_Id_Qualifier { get; set; }

//         public string Interchange_Id_Qualifier { get; set; }

//         public string Interchange_Receiver_Id { get; set; }


//         public string Interchange_Control_Standard_Id_Code { get; set; }

//         public string Interchange_Version { get; set; }

//         public string Interchange_Control_Number { get; set; }

//         public string Acknowledgement_Requested { get; set; }

//         public string Test_Indicator { get; set; }

//         public string Sub_Element_Separator { get; set; }

//         public string DateTime { get; set; } // Computed property for Date and Time combined
//     }

//     public class GS
//     {

//         public string Functional_Identifier_Code { get; set; }

//         public string Functional_SendersCode { get; set; }

//         public string Functional_Receivers_Code { get; set; }
//         public string Group_Control_Number { get; set; }

//         public string Responsible_Agency_Code { get; set; }
//         public string Version { get; set; }

//         public string DateTime { get; set; }
//     }

//     public class ST
//     {

//         public string Transaction_Set_Identifier_Code { get; set; }
//         public string Transaction_Set_Control_Number { get; set; }
//     }

//     public class B4
//     {

//         public string SpecialHandlingCode { get; set; }

//         public string ShipmentStatusCode { get; set; }

//         public string EquipmentStatusCode { get; set; }

//         public string EquipmentType { get; set; }

//         public string ContainerNumber { get; set; }

//         public string DateTime { get; set; }
//     }

//     public class N9
//     {

//         public string Reference_Identification_Qualifier { get; set; }

//         public string Reference_Identification { get; set; }
//     }

//     public class Q2
//     {

//         public string Vessel_Code { get; set; }

//         public string Flight_Number { get; set; }

//         public string Vessel_Name { get; set; }
//     }

//     public class SG
//     {
//         public string Shipment_Status_Code { get; set; }
//         public string DateTime { get; set; } 
        
//     }

//     public class R4
//     {

//         public string Port_Or_Terminal_Function_Code { get; set; }

//         public string Location_Qualifier { get; set; }

//         public string Location_Identifier { get; set; }

//         public string Port_Name { get; set; }
//     }

//     public class SE
//     {

//         public string Number_Of_Included_Segments { get; set; }

//         public string Transaction_Set_Control_Number { get; set; }
//     }

//     public class GE
//     {

//         public string Number_Of_Transaction_Sets_Included { get; set; }
//         public string Group_Control_Number { get; set; }
//     }

//     public class IEA
//     {

//         public string Number_Of_Included_Functional_Groups { get; set; }

//         public string Interchange_Control_Number { get; set; }
//     }
// }
