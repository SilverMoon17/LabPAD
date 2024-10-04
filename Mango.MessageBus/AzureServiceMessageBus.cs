using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Mango.MessageBus;

public class AzureServiceMessageBus : IMessageBus
{
    private readonly string connectionString;

    public AzureServiceMessageBus(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task PublishMessage(BaseMessage msg, string topicName)
    {
        await using var client = new ServiceBusClient(connectionString);

        ServiceBusSender sender = client.CreateSender(topicName);

        var jsonMessage = JsonConvert.SerializeObject(msg);
        ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString()
        };

        await sender.SendMessageAsync(finalMessage);

        await client.DisposeAsync();
    }
}