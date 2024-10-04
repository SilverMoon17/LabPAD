using Mango.Services.ShoppingCartAPI.Models.Dtos;

namespace Mango.Services.ShoppingCartAPI.Repositories.Interfaces;

public interface ICartRepository
{
    Task<CartDto> GetCartByUserIdAsync(string userId);
    Task<CartDto> CreateUpdateCartAsync(CartDto cartDto);
    Task<bool> RemoveFromCartAsync(int cartDetailsId);
    Task<bool> ClearCartAsync(string userId);
    Task<bool> ApplyCoupon(string userId, string couponCode);
    Task<bool> RemoveCoupon(string userId);
}