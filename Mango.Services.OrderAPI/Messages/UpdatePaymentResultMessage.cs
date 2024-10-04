namespace Mango.Services.OrderAPI.Messages;

public class UpdatePaymentResultMessage
{
    public int OrderId { get; set; }
    public bool PaymentStatus { get; set; }
    public string Email { get; set; }
}