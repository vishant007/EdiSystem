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
            Date = DateTimeHelper.ParseDateOnly(lineData.ElementAtOrDefault(4))
        };

        public static void UpdateFeeStatus(MsgData msgData)
        {
            if (msgData.OtherPayments > 0 || msgData.TotalDemurrageFees > 0)
            {
                msgData.FeeStatus = "Pending";
            }
            else
            {
                msgData.FeeStatus = "NA";
            }
        }
        public static N9 ParseN9Segment(string[] lineData, MsgData msgData)
        {
            var n9 = new N9
            {
                ReferenceCode = lineData.ElementAtOrDefault(1)?.Trim(),
                FeeType = lineData.ElementAtOrDefault(2)?.Trim(),
                FeeAmount = decimal.TryParse(lineData.ElementAtOrDefault(2), out var amount) ? amount : 0,
                FeeUntilDate = DateTime.TryParse(lineData.ElementAtOrDefault(3)?.Trim(), out var parsedDate) ? (DateTime?)parsedDate : null,
                FeeUntilTime = TimeSpan.TryParse(lineData.ElementAtOrDefault(4)?.Trim(), out var parsedTime) ? (TimeSpan?)parsedTime : null,
            };
            if (!string.IsNullOrEmpty(n9.FeeType))
            {
                msgData.FeeTypes.Add(n9.FeeType);
            }
            if (n9.ReferenceCode != null)
            {
                if (n9.ReferenceCode.StartsWith("4I"))
                {
                    msgData.TotalDemurrageFees += n9.FeeAmount;
                }
                else if (n9.ReferenceCode == "IGF" || n9.ReferenceCode == "GC")
                {
                    msgData.OtherPayments += n9.FeeAmount;
                }
            }
            UpdateFeeStatus(msgData);

            return n9;
        }


        public static Q2 ParseQ2Segment(string[] lineData) => new Q2
        {
            Vessel_Code = lineData.ElementAtOrDefault(1)?.Trim(),
            Flight_Number = lineData.ElementAtOrDefault(9)?.Trim(),
            Vessel_Name = lineData.ElementAtOrDefault(13)?.Trim()
        };

        public static SG ParseSGSegment(string[] lineData) => new SG
        {
            Shipment_Status_Code = lineData.ElementAtOrDefault(1)?.Trim(),
            Date = DateTimeHelper.ParseDateOnly(lineData.ElementAtOrDefault(4))
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

    }
}
