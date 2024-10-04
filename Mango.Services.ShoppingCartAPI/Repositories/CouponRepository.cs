using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Repositories;

public class CouponRepository : ICouponRepository
{
    private readonly HttpClient _client;

    public CouponRepository(HttpClient client)
    {
        _client = client;
    }

    public async Task<CouponDto> GetCouponAsync(string couponName)
    {
        var response = await _client.GetAsync($"/api/coupon/{couponName}");
        var apiContent = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

        if (resp.IsSuccess)
        {
            return JsonConvert.DeserializeObject<CouponDto>(resp.Result.ToString());
        }

        return new CouponDto();
    }
}