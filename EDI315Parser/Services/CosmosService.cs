using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using EDI315Parser.Models;

namespace EDI315Parser.Services
{
    public static class CosmosService
    {
        private static CosmosClient cosmosClient;
        private static Database database;
        private static Container container;
        private const string databaseId = "EDIParserDatabase";
        private const string containerId = "EDIParserContainer";


        public static async Task InitializeAsync(string endpointUri, string primaryKey)
        {
            cosmosClient = new CosmosClient(endpointUri, primaryKey);
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            container = await database.CreateContainerIfNotExistsAsync(containerId, "/PartitionKey", 400);
            Console.WriteLine("Cosmos DB Initialized.");
        }


        public static async Task AddDataAsync(MsgData msgData)
        {
            try
            {
                msgData.id = Guid.NewGuid().ToString();

                await container.CreateItemAsync(msgData, new PartitionKey(msgData.PartitionKey));
                Console.WriteLine("Data added to Cosmos DB.");
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Cosmos DB Error: {ex.StatusCode} - {ex.Message}");
            }
        }


        public static async Task PushDataToCosmos(MsgData msgData)
        {
            try
            {

                var flatData = new
                {
                    id = msgData.id,
                    PartitionKey = msgData.PartitionKey,
                    TransactionSetIdentifierCode = msgData.stSegment.Transaction_Set_Identifier_Code,
                    TransactionSetControlNumber = msgData.stSegment.Transaction_Set_Control_Number,
                    SpecialHandlingCode = msgData.b4Segment.SpecialHandlingCode,
                    ShipmentStatusCode = msgData.b4Segment.ShipmentStatusCode,
                    EquipmentStatusCode = msgData.b4Segment.EquipmentStatusCode,
                    EquipmentType = msgData.b4Segment.EquipmentType,
                    ContainerNumber = msgData.b4Segment.ContainerNumber,
                    Date = msgData.b4Segment.Date,
                    FeeStatus = msgData.FeeStatus,
                    TotalDemurrageFees = msgData.TotalDemurrageFees,
                    OtherPayments = msgData.OtherPayments,
                    VesselCode = msgData.q2Segment.Vessel_Code,
                    FlightNumber = msgData.q2Segment.Flight_Number,
                    VesselName = msgData.q2Segment.Vessel_Name,
                    ShipmentStatusCodeSG = msgData.sgSegment.Shipment_Status_Code,
                    SGSegmentDate = msgData.sgSegment.Date,
                    LoadingLocation = msgData.r4Segment.Loading_Location,
                    Destination = msgData.r4Segment.Destination,
                    NumberOfIncludedSegments = msgData.seSegment.Number_Of_Included_Segments,
                    SETransactionSetControlNumber = msgData.seSegment.Transaction_Set_Control_Number
                };


                await container.CreateItemAsync(flatData, new PartitionKey(msgData.PartitionKey));
                Console.WriteLine("Flattened data pushed to Cosmos DB.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error pushing data to Cosmos DB: {ex.Message}");
            }
        }

    }
}
