using Mango.Web.Models.CartModels;

namespace Mango.Web.Services.IServices;

public interface ICartService
{
    Task<T> GetCartByUserIdAsync<T>(string userId, string token = null);
    Task<T> AddToCartAsync<T>(CartDto cartDto, string token = null);
    Task<T> UpdateCartAsync<T>(CartDto cartDto, string token = null);
    Task<T> ApplyCoupon<T>(CartDto cartDto, string token = null);
    Task<T> RemoveCoupon<T>(string userId, string token = null);
    Task<T> RemoveFromCartAsync<T>(int cartId, string token = null);
    Task<T> Checkout<T>(CartHeaderDto cartHeaderDto, string token = null);
}