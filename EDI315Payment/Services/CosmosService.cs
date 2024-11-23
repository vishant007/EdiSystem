using EDI315Payment.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDI315Payment.Services
{
    public class CosmosService
    {
        private readonly Container _container;

        public CosmosService(IConfiguration configuration)
        {
            var cosmosClient = new CosmosClient(
                configuration["CosmosDb:Account"],
                configuration["CosmosDb:Key"]
            );
            var database = cosmosClient.GetDatabase(configuration["CosmosDb:DatabaseName"]);
            _container = database.GetContainer(configuration["CosmosDb:ContainerName"]);
        }

        public async Task<MsgData> GetPaymentDetailsAsync(string containerNumber)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.ContainerNumber = @containerNumber")
                .WithParameter("@containerNumber", containerNumber);

            var iterator = _container.GetItemQueryIterator<MsgData>(query);
            if (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                return response.FirstOrDefault();
            }

            return null;
        }

        public async Task UpdateFeeStatusAsync(string id, string partitionKey, string feeStatus)
        {
            // Update only the FeeStatus field using PatchItemAsync
            await _container.PatchItemAsync<MsgData>(
                id,
                new PartitionKey(partitionKey),
                new List<PatchOperation>
                {
                    PatchOperation.Replace("/FeeStatus", feeStatus)
                });
        }

        public async Task UpdateFeeStatusForMultipleContainersAsync(List<UpdatePaymentRequest> requests)
        {
            var tasks = new List<Task>();

            foreach (var request in requests)
            {
                // Fetch payment details for each container
                var paymentDetails = await GetPaymentDetailsAsync(request.ContainerNumber);
                if (paymentDetails != null &&
                    paymentDetails.TotalDemurrageFees == request.TotalDemurrageFees &&
                    paymentDetails.OtherPayments == request.OtherPayments)
                {
                    // Prepare FeeStatus update
                    tasks.Add(UpdateFeeStatusAsync(paymentDetails.id, paymentDetails.PartitionKey, "Paid"));
                }
                else
                {
                    throw new Exception($"Validation failed for ContainerNumber: {request.ContainerNumber}");
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}
