using Mango.Web.Models;
using Mango.Web.Models.CartModels;
using Mango.Web.Models.CouponModels;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly ICouponService _couponService;

    public CartController(ICartService cartService,ICouponService couponService)
    {
        _cartService = cartService;
        _couponService = couponService;
    }

    public async Task<IActionResult> CartIndex()
    {
        return View(await LoadCartDtoBasedOnLoggedInUserAsync());
    }
    
    public async Task<IActionResult> RemoveItem(int cartDetailsId)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        var response = await _cartService.RemoveFromCartAsync<ResponseDto>(cartDetailsId, accessToken);

        if (response is not null && response.IsSuccess)
        {
            return RedirectToAction(nameof(CartIndex));
        }

        return BadRequest();
    }

    public async Task<IActionResult> Checkout()
    {
        
        return View(await LoadCartDtoBasedOnLoggedInUserAsync());
    }
    
    [HttpPost]
    public async Task<IActionResult> Checkout(CartDto cartDto)
    {

        try
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.Checkout<ResponseDto>(cartDto.CartHeader, accessToken);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.DisplayMessage;
                return RedirectToAction(nameof(Checkout));
            }

            return RedirectToAction(nameof(Confirmation));
        }
        catch (Exception e)
        {
            return View(cartDto);
        }
    }
    
    public async Task<IActionResult> Confirmation()
    {
        
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
    {
        var userId = User.Claims.Where(u => u.Type == "sub").FirstOrDefault()?.Value;
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        var response = await _cartService.ApplyCoupon<ResponseDto>(cartDto, accessToken);

        if (response is not null && response.Result is not null && response.IsSuccess)
        {
            return RedirectToAction(nameof(CartIndex));
        }
        
        return RedirectToAction(nameof(CartIndex));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
    {
        var userId = User.Claims.Where(u => u.Type == "sub").FirstOrDefault()?.Value;
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        var response = await _cartService.RemoveCoupon<ResponseDto>(cartDto.CartHeader.UserId, accessToken);

        if (response is not null && response.IsSuccess)
        {
            return RedirectToAction(nameof(CartIndex));
        }

        return BadRequest();
    }
    private async Task<CartDto> LoadCartDtoBasedOnLoggedInUserAsync()
    {
        var userId = User.Claims.Where(u => u.Type == "sub").FirstOrDefault()?.Value;
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, accessToken);

        CartDto cartDto = new CartDto();
        if (response is not null && response.Result is not null && response.IsSuccess)
        {
            cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
        }

        if (cartDto.CartHeader is not null)
        {
            foreach (var detail in cartDto.CartDetails)
            {
                cartDto.CartHeader.OrderTotal += (detail.Count * detail.Product.Price);
            }
            
            if (!cartDto.CartHeader.CouponCode.IsNullOrEmpty())
            {
                var couponRequest = await _couponService.GetCouponAsync<ResponseDto>(cartDto.CartHeader.CouponCode,accessToken);
                if (couponRequest is not null && couponRequest.Result is not null && couponRequest.IsSuccess)
                {
                    var coupon = JsonConvert.DeserializeObject<CouponDto>(couponRequest.Result.ToString());
                    cartDto.CartHeader.DiscountTotal = cartDto.CartHeader.OrderTotal * (coupon.DiscountAmount / 100);
                }
                else
                {
                    TempData["CouponMessage"] = "Coupon is invalid or expired. Please try again.";
                }
            }

            cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
        }

        return cartDto;
    }
}
