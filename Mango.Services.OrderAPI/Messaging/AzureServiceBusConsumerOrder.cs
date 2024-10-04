using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
using Newtonsoft.Json;

namespace Mango.Services.OrderAPI.Messaging;

public class AzureServiceBusConsumerOrder : IAzureServiceBusConsumer
{
    private readonly string _serviceBusConnectionString;
    
    private readonly string _checkoutSubscription;
    private readonly string _checkoutMessageTopic;
    private readonly string _checkoutMessageQueue;
    
    private readonly string _orderPaymentProcessSubscription;
    private readonly string _orderPaymentProcessMessageTopic;
    
    private readonly string _orderUpdatePaymentResultTopic;
    private readonly string _orderUpdatePaymentResultSubscription;

    private ServiceBusProcessor _checkoutProcessor;
    private ServiceBusProcessor _orderUpdatePaymentStatusProcessor;
    
    private readonly OrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IMessageBus _messageBus;

    public AzureServiceBusConsumerOrder(OrderRepository orderRepository, IMapper mapper, IConfiguration configuration, IMessageBus messageBus)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _configuration = configuration;
        _messageBus = messageBus;

        _serviceBusConnectionString = configuration.GetConnectionString("AzureServiceBus");
        
        _checkoutSubscription = configuration.GetValue<string>("CheckoutSubscription");
        _checkoutMessageTopic = configuration.GetValue<string>("CheckoutMessageTopic");
        _checkoutMessageQueue = configuration.GetValue<string>("CheckoutMessageQueue");
        
        _orderPaymentProcessSubscription = configuration.GetValue<string>("OrderPaymentProcessSubscription");
        _orderPaymentProcessMessageTopic = configuration.GetValue<string>("OrderPaymentProcessTopic");

        _orderUpdatePaymentResultSubscription = configuration.GetValue<string>("OrderUpdatePaymentResultSubscription");
        _orderUpdatePaymentResultTopic = configuration.GetValue<string>("OrderUpdatePaymentResultTopic");
        
        var client = new ServiceBusClient(_serviceBusConnectionString);
        // Topic-Subscription method
        // _checkoutProcessor = client.CreateProcessor(_checkoutMessageTopic, _checkoutSubscription);
        
        // Queue method
        _checkoutProcessor = client.CreateProcessor(_checkoutMessageQueue);
        _orderUpdatePaymentStatusProcessor = client.CreateProcessor(_orderUpdatePaymentResultTopic, _orderUpdatePaymentResultSubscription);
    }

    public async Task Start()
    {
        _checkoutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
        _checkoutProcessor.ProcessErrorAsync += ErrorHandler;
        await _checkoutProcessor.StartProcessingAsync();
        
        _orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
        _orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
        await _orderUpdatePaymentStatusProcessor.StartProcessingAsync();
    }


    public async Task Stop()
    {
        _checkoutProcessor.ProcessMessageAsync -= OnCheckOutMessageReceived;
        _checkoutProcessor.ProcessErrorAsync -= ErrorHandler;
        await _checkoutProcessor.StopProcessingAsync();
        await _checkoutProcessor.DisposeAsync();
        
        _orderUpdatePaymentStatusProcessor.ProcessMessageAsync -= OnOrderPaymentUpdateReceived;
        _orderUpdatePaymentStatusProcessor.ProcessErrorAsync -= ErrorHandler;
        await _orderUpdatePaymentStatusProcessor.StopProcessingAsync();
        await _orderUpdatePaymentStatusProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        Console.WriteLine(arg.Exception.ToString());
        return Task.CompletedTask;
    }

    private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);
        CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);
        OrderHeader orderHeader = new()
        {
            UserId = checkoutHeaderDto.UserId,
            FirstName = checkoutHeaderDto.FirstName,
            LastName = checkoutHeaderDto.LastName,
            OrderDetails = new List<OrderDetails>(),
            CardNumber = checkoutHeaderDto.CardNumber,
            CouponCode = checkoutHeaderDto.CouponCode,
            CVV = checkoutHeaderDto.CVV,
            DiscountTotal = checkoutHeaderDto.DiscountTotal,
            Email = checkoutHeaderDto.Email,
            ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
            OrderTime = DateTime.Now,
            OrderTotal = checkoutHeaderDto.OrderTotal,
            PaymentStatus = false,
            Phone = checkoutHeaderDto.Phone,
            PickupDateTime = checkoutHeaderDto.PickupDateTime
        };
        foreach(var detailList in checkoutHeaderDto.CartDetails)
        {
            OrderDetails orderDetails = new()
            {
                ProductId = detailList.ProductId,
                ProductName = detailList.Product.Name,
                Price = detailList.Product.Price,
                Count = detailList.Count
            };
            orderHeader.CartTotalItems += detailList.Count;
            orderHeader.OrderDetails.Add(orderDetails);
        }
        await _orderRepository.AddOrderAsync(orderHeader);

        PaymentRequestMessage paymentRequestMessage = new()
        {
            Name = $"{orderHeader.FirstName} {orderHeader.LastName}",
            Email = orderHeader.Email,
            CardNumber = orderHeader.CardNumber,
            CVV = orderHeader.CVV,
            OrderId = orderHeader.OrderHeaderId,
            OrderTotal = orderHeader.OrderTotal
        };

        try
        {
            await _messageBus.PublishMessage(paymentRequestMessage, _orderPaymentProcessMessageTopic);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception e)
        {
            throw;
        }
    }
    private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs arg)
    {
        var message = arg.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        UpdatePaymentResultMessage updatePaymentResultMessage =
            JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

        await _orderRepository.UpdateOrderPaymentStatusAsync(updatePaymentResultMessage.OrderId, updatePaymentResultMessage.PaymentStatus);

        await arg.CompleteMessageAsync(message);
    }
}
