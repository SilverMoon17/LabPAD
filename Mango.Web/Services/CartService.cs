using Mango.Web.Models;
using Mango.Web.Models.CartModels;

namespace Mango.Web.Services.IServices;

public class CartService : BaseService, ICartService
{
    private readonly IHttpClientFactory _clientFactory;

    public CartService(IHttpClientFactory clientFactory) : base(clientFactory)
    {
        _clientFactory = clientFactory;
    }
    
    public async Task<T> GetCartByUserIdAsync<T>(string userId, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = SD.ShoppingCartAPIBase + "/api/cart/GetCart/" + userId,
            AccessToken = token
        });
    }

    public async Task<T> AddToCartAsync<T>(CartDto cartDto, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPIBase + "/api/cart/AddCart",
            AccessToken = token
        });
    }

    public async Task<T> UpdateCartAsync<T>(CartDto cartDto, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.PUT,
            Data = cartDto,
            Url = SD.ShoppingCartAPIBase + "/api/cart/UpdateCart",
            AccessToken = token
        });
    }

    public async Task<T> ApplyCoupon<T>(CartDto cartDto, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPIBase + "/api/cart/ApplyCoupon",
            AccessToken = token
        });
    }

    public async Task<T> RemoveCoupon<T>(string userId, string token)
    {
        return await this.SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = userId,
            Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCoupon",
            AccessToken = token
        });
    }

    public async Task<T> RemoveFromCartAsync<T>(int cartId, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.DELETE,
            Data = cartId,
            Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCart",
            AccessToken = token
        });
    }

    public async Task<T> Checkout<T>(CartHeaderDto cartHeaderDto, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = cartHeaderDto,
            Url = SD.ShoppingCartAPIBase + "/api/cart/Checkout",
            AccessToken = token
        });
    }
}