using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace EDI315Api.Services
{
    public class CosmosDbService
    {
        private readonly Container _container;

        public CosmosDbService(IConfiguration configuration)
        {
            var cosmosClient = new CosmosClient(
                configuration["CosmosDb:Account"],
                configuration["CosmosDb:Key"]
            );

            var database = cosmosClient.GetDatabase(configuration["CosmosDb:DatabaseName"]);
            _container = database.GetContainer(configuration["CosmosDb:ContainerName"]);
        }

        // Method to fetch all items with full details
        public async Task<List<Dictionary<string, object>>> GetAllItemsAsync()
        {
            var query = "SELECT * FROM c";
            var queryIterator = _container.GetItemQueryIterator<JObject>(new QueryDefinition(query));

            var results = new List<Dictionary<string, object>>();

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();

                foreach (var item in response)
                {
                    var itemData = new Dictionary<string, object>();

                    foreach (var property in item.Properties())
                    {
                        if (property.Value is JObject nestedObject)
                        {
                            var nestedData = new Dictionary<string, object>();
                            foreach (var nestedProperty in nestedObject.Properties())
                            {
                                nestedData[nestedProperty.Name] = nestedProperty.Value.Type == JTokenType.Null
                                                                  ? "NULL"
                                                                  : nestedProperty.Value.ToObject<object>();
                            }
                            itemData[property.Name] = nestedData;
                        }
                        else
                        {
                            itemData[property.Name] = property.Value.Type == JTokenType.Null
                                                      ? "NULL"
                                                      : property.Value.ToObject<object>();
                        }
                    }

                    results.Add(itemData);
                }
            }

            return results;
        }


        // Method specifically to fetch only ContainerNumber for the Watchlist
        public async Task<List<string>> GetContainerNumbersForWatchlistAsync()
        {
            var query = "SELECT c.b4Segment.ContainerNumber FROM c";
            var queryIterator = _container.GetItemQueryIterator<JObject>(new QueryDefinition(query));

            var containerNumbers = new List<string>();

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();

                foreach (var item in response)
                {
                    var containerNumber = item.SelectToken("b4Segment.ContainerNumber")?.ToString();
                    if (!string.IsNullOrEmpty(containerNumber))
                    {
                        containerNumbers.Add(containerNumber);
                    }
                }
            }

            return containerNumbers;
        }
        public async Task<Dictionary<string, object>> GetContainerDetailsAsync(string containerNumber)
        {
            var query = $"SELECT * FROM c WHERE c.ContainerNumber = @containerNumber";
            var queryIterator = _container.GetItemQueryIterator<JObject>(new QueryDefinition(query)
                .WithParameter("@containerNumber", containerNumber));

            if (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                var containerDetails = response.FirstOrDefault();

                if (containerDetails != null)
                {
                    var detailsData = new Dictionary<string, object>();
                    foreach (var property in containerDetails.Properties())
                    {
                        detailsData[property.Name] = property.Value.Type == JTokenType.Null ? "NULL" : property.Value.ToObject<object>();
                    }
                    return detailsData;
                }
            }

            return null;
        }


    }
}
