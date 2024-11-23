using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Threading.Tasks;

namespace EDI315Payment.Services
{
    public class AzureServiceBusService
    {
        private readonly ServiceBusClient _client;
        private readonly string _topicName;

        public AzureServiceBusService(IConfiguration configuration)
        {
            _client = new ServiceBusClient(configuration["ServiceBus:ConnectionString"]);
            _topicName = configuration["ServiceBus:TopicName"];
        }

        public async Task SendMessageAsync(object message)
        {
            var sender = _client.CreateSender(_topicName);

            var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(message))
            {
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
