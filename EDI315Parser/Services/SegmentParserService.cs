using System;
using EDI315Parser.Models;
using EDI315Parser.Helpers;

namespace EDI315Parser.Services
{
    public static class SegmentParserService
    {
        public static ST ParseSTSegment(string[] lineData) => new ST
        {
            Transaction_Set_Identifier_Code = lineData.ElementAtOrDefault(1)?.Trim(),
            Transaction_Set_Control_Number = lineData.ElementAtOrDefault(2)?.Trim()
        };

        public static B4 ParseB4Segment(string[] lineData) => new B4
        {
            SpecialHandlingCode = lineData.ElementAtOrDefault(1)?.Trim(),
            ShipmentStatusCode = lineData.ElementAtOrDefault(3)?.Trim(),
            EquipmentStatusCode = lineData.ElementAtOrDefault(8)?.Trim(),
            EquipmentType = lineData.ElementAtOrDefault(9)?.Trim(),
            ContainerNumber = $"{lineData.ElementAtOrDefault(7)?.Trim()}{lineData.ElementAtOrDefault(8)?.Trim()}",
            Date = DateTimeHelper.ParseDateOnly(lineData.ElementAtOrDefault(4))  // Use helper method here
        };

        public static N9 ParseN9Segment(string[] lineData) => new N9
        {
            Reference_Identification_Qualifier = lineData.ElementAtOrDefault(1)?.Trim(),
            Reference_Identification = lineData.ElementAtOrDefault(2)?.Trim()
        };

        public static Q2 ParseQ2Segment(string[] lineData) => new Q2
        {
            Vessel_Code = lineData.ElementAtOrDefault(1)?.Trim(),
            Flight_Number = lineData.ElementAtOrDefault(9)?.Trim(),
            Vessel_Name = lineData.ElementAtOrDefault(13)?.Trim()
        };

        public static SG ParseSGSegment(string[] lineData) => new SG
        {
            Shipment_Status_Code = lineData.ElementAtOrDefault(1)?.Trim(),
            Date = DateTimeHelper.ParseDateOnly(lineData.ElementAtOrDefault(4))  // Use helper method here
        };

        public static R4 ParseR4Segment(string[] lineData, R4 r4)
        {
            if (lineData.Length > 4)
            {
                if (lineData[1] == "L")
                {
                    r4.Loading_Location = lineData.ElementAtOrDefault(4)?.Trim();
                }
                else if (lineData[1] == "D")
                {
                    r4.Destination = lineData.ElementAtOrDefault(4)?.Trim();
                }
            }

            return r4;
        }

        public static SE ParseSESegment(string[] lineData) => new SE
        {
            Number_Of_Included_Segments = int.TryParse(lineData.ElementAtOrDefault(1)?.Trim(), out var count) ? count : 0,
            Transaction_Set_Control_Number = lineData.ElementAtOrDefault(2)?.Trim()
        };

        public static GE ParseGESegment(string[] lineData) => new GE
        {
            Number_Of_Transaction_Sets_Included = int.TryParse(lineData.ElementAtOrDefault(1)?.Trim(), out var count) ? count : 0,
            Group_Control_Number = lineData.ElementAtOrDefault(2)?.Trim()
        };

        public static IEA ParseIEASegment(string[] lineData) => new IEA
        {
            Number_Of_Included_Functional_Groups = int.TryParse(lineData.ElementAtOrDefault(1)?.Trim(), out var count) ? count : 0,
            Interchange_Control_Number = lineData.ElementAtOrDefault(2)?.Trim()
        };
    }
}
