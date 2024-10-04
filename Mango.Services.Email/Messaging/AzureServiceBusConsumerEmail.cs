using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Mango.Services.Email.Repository;
using Newtonsoft.Json;
using Mango.Services.Email.Messages;

namespace Mango.Services.Email.Messaging;

public class AzureServiceBusConsumerEmail : IAzureServiceBusConsumer
{
    private readonly string _serviceBusConnectionString;
    private readonly string _emailSubscription;
    private readonly string _orderUpdatePaymentResultTopic;

    private readonly EmailRepository _emailRepository;

    private ServiceBusProcessor _orderUpdatePaymentStatusProcessor;

    private readonly IConfiguration _configuration;

    public AzureServiceBusConsumerEmail(EmailRepository emailRepository, IConfiguration configuration)
    {
        _emailRepository = emailRepository;
        _configuration = configuration;

        _serviceBusConnectionString = _configuration.GetConnectionString("AzureServiceBus");;
        _emailSubscription = _configuration.GetValue<string>("EmailSubscription");
        _orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");


        var client = new ServiceBusClient(_serviceBusConnectionString);

        _orderUpdatePaymentStatusProcessor = client.CreateProcessor(_orderUpdatePaymentResultTopic, _emailSubscription);
    }

    public async Task Start()
    {
      
        _orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
        _orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
        await _orderUpdatePaymentStatusProcessor.StartProcessingAsync();
    }
    public async Task Stop()
    {
        
        await _orderUpdatePaymentStatusProcessor.StopProcessingAsync();
        await _orderUpdatePaymentStatusProcessor.DisposeAsync();
    }
    Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }



    private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        UpdatePaymentResultMessage objMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);
        try
        {
            await _emailRepository.SendAndLogEmail(objMessage);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception e)
        {
            throw;
        }
    }
}
