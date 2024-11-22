using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CosmosService
{
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;

    public CosmosService(string accountUri, string accountKey, string databaseName, string containerName)
    {
        _cosmosClient = new CosmosClient(accountUri, accountKey);
        var database = _cosmosClient.GetDatabase(databaseName);
        _container = database.GetContainer(containerName);
    }

    // Fetch all payments
    public async Task<List<Payment>> GetPaymentsAsync()
    {
        var query = _container.GetItemQueryIterator<Payment>(
            "SELECT c.id, c.ContainerNumber, c.FeeStatus, c.TotalDemurrageFees, c.OtherPayments FROM c"
        );

        List<Payment> payments = new List<Payment>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            payments.AddRange(response);
        }

        return payments;
    }

    // Fetch payment by container number
    public async Task<Payment> GetPaymentByContainerAsync(string containerNumber)
    {
        var query = _container.GetItemQueryIterator<Payment>(
            $"SELECT c.id, c.ContainerNumber, c.FeeStatus, c.TotalDemurrageFees, c.OtherPayments FROM c WHERE c.ContainerNumber = '{containerNumber}'"
        );

        if (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            return response.FirstOrDefault(); // Return the first matching record or null if not found
        }

        return null;
    }

    
}
