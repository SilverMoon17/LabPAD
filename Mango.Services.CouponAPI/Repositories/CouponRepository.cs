using AutoMapper;
using Mango.Services.CouponAPI.DbContexts;
using Mango.Services.CouponAPI.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Repositories;

public class CouponRepository : ICouponRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public CouponRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CouponDto> GetCouponByCode(string couponCode)
    {
        var coupon = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.CouponCode == couponCode);

        return _mapper.Map<CouponDto>(coupon);
    }
}