using Mango.Services.CouponAPI.Models.Dtos;
using Mango.Services.CouponAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers;

[ApiController]
[Route("api/coupon")]
public class CouponAPIController : ControllerBase
{
    private readonly ICouponRepository _couponRepository;

    public CouponAPIController(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }
    
    [HttpGet("{couponCode}")]
    public async Task<object> GetDiscountForCode(string couponCode)
    {
        var response = new ResponseDto();
        try
        {
            CouponDto couponDto = await _couponRepository.GetCouponByCode(couponCode);
            response.Result = couponDto;
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return response;
    }
}