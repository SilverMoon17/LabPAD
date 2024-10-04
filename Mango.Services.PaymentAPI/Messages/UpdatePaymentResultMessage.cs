using Mango.MessageBus;

namespace Mango.Services.PaymentAPI.Messages;

public class UpdatePaymentResultMessage : BaseMessage
{
    public int OrderId { get; set; }
    public bool PaymentStatus { get; set; }
    public string Email { get; set; }
}