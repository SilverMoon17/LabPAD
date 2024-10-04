using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Messages;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers;

[ApiController]
[Route("api/cart")]
public class CartAPIController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IMessageBus _messageBus;

    public CartAPIController(ICartRepository cartRepository, IMessageBus messageBus, ICouponRepository couponRepository)
    {
        _cartRepository = cartRepository;
        _messageBus = messageBus;
        _couponRepository = couponRepository;
    }

    [HttpGet("GetCart/{userId}")]
    public async Task<object> GetCart(string userId)
    {
        var response = new ResponseDto();
        try
        {
            CartDto cartDto = await _cartRepository.GetCartByUserIdAsync(userId);
            response.Result = cartDto;
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return response;
    }

    [HttpPost("AddCart")]
    public async Task<object> AddCart(CartDto model)
    {
        var response = new ResponseDto();
        try
        {
            CartDto cartDto = await _cartRepository.CreateUpdateCartAsync(model);
            response.Result = cartDto;
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return response;
    }

    [HttpPut("UpdateCart")]
    public async Task<object> UpdateCart(CartDto model)
    {
        var response = new ResponseDto();
        try
        {
            CartDto cartDto = await _cartRepository.CreateUpdateCartAsync(model);
            response.Result = cartDto;
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return response;
    }

    [HttpDelete("RemoveCart")]
    public async Task<object> RemoveCart([FromBody] int cartId)
    {
        var response = new ResponseDto();
        try
        {
            bool isSuccess = await _cartRepository.RemoveFromCartAsync(cartId);
            response.Result = isSuccess;
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return response;
    }
    
    [HttpPost("ApplyCoupon")]
    public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
    {
        var response = new ResponseDto();
        try
        {
            bool isSuccess = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
            response.Result = isSuccess;
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return response;
    }
    
    [HttpPost("RemoveCoupon")]
    public async Task<object> RemoveCoupon([FromBody] string userId)
    {
        var response = new ResponseDto();
        try
        {
            bool isSuccess = await _cartRepository.RemoveCoupon(userId);
            response.Result = isSuccess;
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return response;
    }
    
    [HttpPost("Checkout")]
    public async Task<object> Checkout(CheckoutHeaderDto checkoutHeaderDto)
    {
        var response = new ResponseDto();
        try
        {
            CartDto cartDto = await _cartRepository.GetCartByUserIdAsync(checkoutHeaderDto.UserId);

            if (cartDto is null)
            {
                return BadRequest();
            }

            if (!string.IsNullOrEmpty(checkoutHeaderDto.CouponCode))
            {
                CouponDto couponDto = await _couponRepository.GetCouponAsync(checkoutHeaderDto.CouponCode);

                var currentDiscount = (checkoutHeaderDto.DiscountTotal / (checkoutHeaderDto.OrderTotal + checkoutHeaderDto.DiscountTotal)) * 100;
                if (Math.Abs(currentDiscount - couponDto.DiscountAmount) > 0.001)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string> { "Coupon discount has changed, please confirm!"};
                    response.DisplayMessage = "Coupon discount has changed, please confirm!";
                    return response;
                }
            }

            checkoutHeaderDto.CartDetails = cartDto.CartDetails;
            // logic to add message to process order
            await _messageBus.PublishMessage(checkoutHeaderDto, "checkoutqueue");

            await _cartRepository.ClearCartAsync(checkoutHeaderDto.UserId);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return response;
    }
}
