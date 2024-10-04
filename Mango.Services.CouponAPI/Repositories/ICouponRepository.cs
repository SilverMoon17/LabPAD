using Mango.Services.CouponAPI.Models.Dtos;

namespace Mango.Services.CouponAPI.Repositories;

public interface ICouponRepository
{
    Task<CouponDto> GetCouponByCode(string couponCode);
    // TODO: Add CRUD operations for coupons
}