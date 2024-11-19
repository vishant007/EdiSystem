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

        // Initializes CosmosClient and Container
        public static async Task InitializeAsync(string endpointUri, string primaryKey)
        {
            cosmosClient = new CosmosClient(endpointUri, primaryKey);
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            container = await database.CreateContainerIfNotExistsAsync(containerId, "/PartitionKey", 400);
            Console.WriteLine("Cosmos DB Initialized.");
        }

        // Adds data to Cosmos DB using the PartitionKey from MsgData
        public static async Task AddDataAsync(MsgData msgData)
        {
            try
            {
                msgData.id = Guid.NewGuid().ToString();  // Generate new ID for each document
                // Create the item in Cosmos DB with the correct PartitionKey
                await container.CreateItemAsync(msgData, new PartitionKey(msgData.PartitionKey));
                Console.WriteLine("Data added to Cosmos DB.");
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Cosmos DB Error: {ex.StatusCode} - {ex.Message}");
            }
        }

        // Push data to Cosmos DB with the PartitionKey from MsgData
        public static async Task PushDataToCosmos(MsgData msgData)
        {
            try
            {
                // Push data using PartitionKey from MsgData class
                await container.CreateItemAsync(msgData, new PartitionKey(msgData.PartitionKey));
                Console.WriteLine("Data pushed to Cosmos DB.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error pushing data to Cosmos DB: {ex.Message}");
            }
        }
    }
}
