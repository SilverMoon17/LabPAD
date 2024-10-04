namespace Mango.Web.Models.CartModels;

public class CartDto
{
    public CartHeaderDto CartHeader { get; set; }
    public IEnumerable<CartDetailsDto> CartDetails { get; set; }
}