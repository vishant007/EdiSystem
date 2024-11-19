using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using EDI315Parser.Models;
using System.Text.Json.Serialization;

public class Program
{
    private static CosmosClient cosmosClient;
    private static Database database;
    private static Container container;
    private static string cosmosEndpointUri = "https://vishant79.documents.azure.com:443/";
    private static string cosmosPrimaryKey = "C8TRZFpkXaTL5BVXWre2vIBqGWVFQJNq5Iwv7qZqQmnQb4321fcFkKdvqst7vt2BAHjQJ4AanZZfACDbQ0wHTw==";
    private static string databaseId = "EDIParserDatabase";
    private static string containerId = "EDIParserContainer";

    private static async Task Main(string[] args)
    {
        string inputFilePath = "D:/project-docs/70239949-O20241025715250.txt";
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        cosmosClient = new CosmosClient(cosmosEndpointUri, cosmosPrimaryKey);
        await CreateDatabaseAndContainerAsync();

        string[] fileData = File.ReadAllLines(inputFilePath);
        MsgData msgData = null;

        foreach (string line in fileData)
        {
            string[] lineData = line.Split('*');

            switch (lineData[0])
            {
                case "ISA":
                    if (msgData == null) msgData = new MsgData();
                    msgData.isaSegment = ParseISASegment(lineData);
                    break;

                case "GS":
                    if (msgData != null) msgData.gsSegment = ParseGSSegment(lineData);
                    break;

                case "ST":
                    msgData = new MsgData();
                    msgData.stSegment = ParseSTSegment(lineData);
                    break;

                case "B4":
                    if (msgData != null) msgData.b4Segment = ParseB4Segment(lineData);
                    break;

                case "N9":
                    if (msgData != null) msgData.n9Segment = ParseN9Segment(lineData);
                    break;

                case "Q2":
                    if (msgData != null) msgData.q2Segment = ParseQ2Segment(lineData);
                    break;

                case "SG":
                    if (msgData != null) msgData.sgSegment = ParseSGSegment(lineData);
                    break;

                case "R4":
                    if (msgData != null) msgData.r4Segment = ParseR4Segment(lineData);
                    break;

                case "SE":
                    if (msgData != null)
                    {
                        msgData.seSegment = ParseSESegment(lineData);
                        await PushDataToCosmos(msgData);
                        msgData = null;
                    }
                    break;

                case "GE":
                    if (msgData != null) msgData.geSegment = ParseGESegment(lineData);
                    break;

                case "IEA":
                    if (msgData != null) msgData.ieaSegment = ParseIEASegment(lineData);
                    break;
            }
        }

        Console.WriteLine("All segments processed and data pushed to Cosmos DB.");
    }

    private static async Task CreateDatabaseAndContainerAsync()
    {
        try
        {
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Database created or opened successfully.");

            container = await database.CreateContainerIfNotExistsAsync(containerId, "/PartitionKey", 400);
            Console.WriteLine("Container created or opened successfully.");
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Cosmos DB Error during creation: {ex.StatusCode} - {ex.Message}");
        }
    }

    private static async Task PushDataToCosmos(MsgData msgData)
    {
        try
        {
            msgData.id = Guid.NewGuid().ToString();
            await container.CreateItemAsync(msgData);
            Console.WriteLine("Segment data successfully pushed to Cosmos DB.");
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Cosmos DB Error: {ex.StatusCode} - {ex.Message}");
        }
    }

    private static ISA ParseISASegment(string[] lineData)
    {
        ISA isa = new ISA();
        if (lineData.Length > 1) isa.Authorization_Information_Qualifier = lineData[1].Trim();
        if (lineData.Length > 2) isa.Secutity_Information_Qualifier = lineData[2].Trim();
        if (lineData.Length > 3) isa.Interchange_Id_Qualifier_Sender = lineData[3].Trim();
        if (lineData.Length > 4) isa.Interchange_Sender_Id_Qualifier = lineData[4].Trim();
        if (lineData.Length > 5) isa.Interchange_Id_Qualifier = lineData[5].Trim();
        if (lineData.Length > 6) isa.Interchange_Receiver_Id = lineData[6].Trim();
        if (lineData.Length > 7) isa.Interchange_Control_Standard_Id_Code = lineData[7].Trim();
        if (lineData.Length > 8) isa.Interchange_Version = lineData[8].Trim();
        if (lineData.Length > 9) isa.Interchange_Control_Number = lineData[9].Trim();
        if (lineData.Length > 10) isa.Acknowledgement_Requested = ParseBoolean(lineData[10]);
        if (lineData.Length > 11) isa.Test_Indicator = ParseBoolean(lineData[11]);
        if (lineData.Length > 12) isa.Sub_Element_Separator = lineData[12].Trim();
        if (lineData.Length > 13) isa.DateTime = ParseDateTime(lineData[13]) ?? DateTime.MinValue;
        return isa;
    }

    private static GS ParseGSSegment(string[] lineData)
    {
        GS gs = new GS();
        if (lineData.Length > 1) gs.Functional_Identifier_Code = lineData[1].Trim();
        if (lineData.Length > 2) gs.Functional_SendersCode = lineData[2].Trim();
        if (lineData.Length > 3) gs.Functional_Receivers_Code = lineData[3].Trim();
        if (lineData.Length > 4) gs.Group_Control_Number = lineData[4].Trim();
        if (lineData.Length > 5) gs.Responsible_Agency_Code = lineData[5].Trim();
        if (lineData.Length > 6) gs.Version = lineData[6].Trim();
        if (lineData.Length > 7) gs.DateTime = ParseDateTime(lineData[7]) ?? DateTime.MinValue; // Default to MinValue if null
        return gs;
    }

    private static ST ParseSTSegment(string[] lineData)
    {
        ST st = new ST();
        if (lineData.Length > 1) st.Transaction_Set_Identifier_Code = lineData[1].Trim();
        if (lineData.Length > 2) st.Transaction_Set_Control_Number = lineData[2].Trim();
        return st;
    }

    private static B4 ParseB4Segment(string[] lineData)
    {
        B4 b4 = new B4();
        if (lineData.Length > 1) b4.SpecialHandlingCode = lineData[1].Trim();
        if (lineData.Length > 3) b4.ShipmentStatusCode = lineData[3].Trim();
        if (lineData.Length > 8) b4.EquipmentStatusCode = lineData[8].Trim();
        if (lineData.Length > 9) b4.EquipmentType = lineData[9].Trim();
        if (lineData.Length > 7) b4.ContainerNumber = $"{lineData[7].Trim()}{lineData[8].Trim()}";
        if (lineData.Length > 4 && lineData.Length > 5) b4.DateTime = ParseDateTime($"{lineData[4]} {lineData[5]}") ?? DateTime.MinValue; // Default to MinValue if null
        return b4;
    }

    private static N9 ParseN9Segment(string[] lineData)
    {
        N9 n9 = new N9();
        if (lineData.Length > 1) n9.Reference_Identification_Qualifier = lineData[1].Trim();
        if (lineData.Length > 2) n9.Reference_Identification = lineData[2].Trim();
        return n9;
    }

    private static Q2 ParseQ2Segment(string[] lineData)
    {
        Q2 q2 = new Q2();
        if (lineData.Length > 1) q2.Vessel_Code = lineData[1].Trim();
        if (lineData.Length > 9) q2.Flight_Number = lineData[9].Trim();
        if (lineData.Length > 13) q2.Vessel_Name = lineData[13].Trim();
        return q2;
    }

    private static SG ParseSGSegment(string[] lineData)
    {
        SG sg = new SG();
        if (lineData.Length > 1) sg.Shipment_Status_Code = lineData[1].Trim();
        if (lineData.Length > 4 && lineData.Length > 5) sg.DateTime = ParseDateTime($"{lineData[4]} {lineData[5]}") ?? DateTime.MinValue; // Default to MinValue if null
        return sg;
    }

    private static R4 ParseR4Segment(string[] lineData)
    {
        R4 r4 = new R4();
        if (lineData.Length > 1) r4.Port_Or_Terminal_Function_Code = lineData[1].Trim();
        if (lineData.Length > 2) r4.Location_Qualifier = lineData[2].Trim();
        if (lineData.Length > 3) r4.Location_Identifier = lineData[3].Trim();
        if (lineData.Length > 4) r4.Port_Name = lineData[4].Trim();
        return r4;
    }

    private static SE ParseSESegment(string[] lineData)
    {
        SE se = new SE();
        if (lineData.Length > 1) se.Number_Of_Included_Segments = int.TryParse(lineData[1].Trim(), out int count) ? count : 0;
        if (lineData.Length > 2) se.Transaction_Set_Control_Number = lineData[2].Trim();
        return se;
    }

    private static GE ParseGESegment(string[] lineData)
    {
        GE ge = new GE();
        if (lineData.Length > 1) ge.Number_Of_Transaction_Sets_Included = int.TryParse(lineData[1].Trim(), out int count) ? count : 0;
        if (lineData.Length > 2) ge.Group_Control_Number = lineData[2].Trim();
        return ge;
    }

    private static IEA ParseIEASegment(string[] lineData)
    {
        IEA iea = new IEA();
        if (lineData.Length > 1) iea.Number_Of_Included_Functional_Groups = int.TryParse(lineData[1].Trim(), out int count) ? count : 0;
        if (lineData.Length > 2) iea.Interchange_Control_Number = lineData[2].Trim();
        return iea;
    }

    // Helper method to parse DateTime from a string
    private static DateTime? ParseDateTime(string dateTimeStr)
    {
        if (DateTime.TryParse(dateTimeStr, out DateTime result))
        {
            return result;
        }
        return null;
    }

    private static bool ParseBoolean(string boolStr)
    {
        return bool.TryParse(boolStr, out bool result) && result;
    }
}
