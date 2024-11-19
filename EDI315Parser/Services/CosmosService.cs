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
