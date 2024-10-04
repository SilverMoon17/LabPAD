using System.Text;
using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.PaymentAPI.Messages;
using Newtonsoft.Json;
using PaymentProcessor;

namespace Mango.Services.PaymentAPI.Messaging;

public class AzureServiceBusConsumerPayment : IAzureServiceBusConsumer
{
    private readonly string _serviceBusConnectionString; 
    
    private readonly string _orderPaymentProcessSubscription;
    private readonly string _orderPaymentProcessMessageTopic;
    
    private readonly string _orderUpdatePaymentResultTopic;
    private readonly string _orderUpdatePaymentResultSubscription;

    private readonly IProcessPayment _processPayment;

    private ServiceBusProcessor _orderPaymentProcessor;
    private readonly IConfiguration _configuration;
    private readonly IMessageBus _messageBus;

    public AzureServiceBusConsumerPayment(IConfiguration configuration, IMessageBus messageBus, IProcessPayment processPayment)
    {
        _configuration = configuration;
        _messageBus = messageBus;
        _processPayment = processPayment;

        _serviceBusConnectionString = configuration.GetConnectionString("AzureServiceBus");
        
        _orderPaymentProcessSubscription = configuration.GetValue<string>("OrderPaymentProcessSubscription");
        _orderPaymentProcessMessageTopic = configuration.GetValue<string>("OrderPaymentProcessTopic");
        
        _orderUpdatePaymentResultSubscription = configuration.GetValue<string>("OrderUpdatePaymentResultSubscription");
        _orderUpdatePaymentResultTopic = configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

        var client = new ServiceBusClient(_serviceBusConnectionString);
        _orderPaymentProcessor = client.CreateProcessor(_orderPaymentProcessMessageTopic, _orderPaymentProcessSubscription);
    }

    public async Task Start()
    {
        _orderPaymentProcessor.ProcessMessageAsync += ProcessPayment;
        _orderPaymentProcessor.ProcessErrorAsync += ErrorHandler;
        await _orderPaymentProcessor.StartProcessingAsync();
    }
    
    public async Task Stop()
    {
        _orderPaymentProcessor.ProcessMessageAsync -= ProcessPayment;
        _orderPaymentProcessor.ProcessErrorAsync -= ErrorHandler;
        await _orderPaymentProcessor.StopProcessingAsync();
        await _orderPaymentProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        Console.WriteLine(arg.Exception.ToString());
        return Task.CompletedTask;
    }

    private async Task ProcessPayment(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);

        var result = _processPayment.PaymentProcessor();

        UpdatePaymentResultMessage updatePaymentResultMessage = new()
        {
            PaymentStatus = result,
            OrderId = paymentRequestMessage.OrderId,
            Email = paymentRequestMessage.Email
        };

        await _messageBus.PublishMessage(updatePaymentResultMessage, _orderUpdatePaymentResultTopic);
        await args.CompleteMessageAsync(message);
    }
}