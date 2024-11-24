using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace EDI315Api.Services
{
    public class AzureServiceBusService
    {
        private readonly string _connectionString;
        private readonly string _topicName;

        public AzureServiceBusService(IConfiguration configuration)
        {
            _connectionString = configuration["ServiceBus:ConnectionString"];
            _topicName = configuration["ServiceBus:TopicName"];
        }

        public async Task SendMessageAsync(object messageContent)
        {
            await using var client = new ServiceBusClient(_connectionString);
            var sender = client.CreateSender(_topicName);

            var message = new ServiceBusMessage(JsonSerializer.Serialize(messageContent))
            {
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(message);
        }
    }
}
