using Mango.Services.ShoppingCartAPI.Models.Dtos;

namespace Mango.Services.ShoppingCartAPI.Repositories.Interfaces;

public interface ICouponRepository
{
    Task<CouponDto> GetCouponAsync(string couponName);
}