using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Threading.Tasks;
using EDI315Payment.Models;

namespace EDI315Payment.Services
{
    public class AzureServiceBusService
    {
        private readonly ServiceBusClient _client;
        private readonly string _topicName;
        private readonly string _subscriptionName;

        public AzureServiceBusService(IConfiguration configuration)
        {
            var connectionString = configuration["ServiceBus:ConnectionString"];
            _topicName = configuration["ServiceBus:TopicName"];
            _subscriptionName = configuration["ServiceBus:SubscriptionName"];
            _client = new ServiceBusClient(connectionString);
        }

        public async Task<MsgData> ReceiveMessageAsync()
        {
            var receiver = _client.CreateReceiver(_topicName, _subscriptionName);

            // Receive a single message
            var message = await receiver.ReceiveMessageAsync();
            if (message != null)
            {
                var messageBody = message.Body.ToString();
                var msgData = JsonSerializer.Deserialize<MsgData>(messageBody);

                // Complete the message to remove it from the queue
                await receiver.CompleteMessageAsync(message);
                return msgData;
            }

            return null;
        }

        public async Task SendMessageAsync(object message)
        {
            var sender = _client.CreateSender(_topicName);
            var messageJson = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(messageJson);

            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
