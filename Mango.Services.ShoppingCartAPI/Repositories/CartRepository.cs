using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public CartRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CartDto> GetCartByUserIdAsync(string userId)
    {
        var cartHeaderFromDb = await _dbContext.CartHeaders.FirstOrDefaultAsync(ch => ch.UserId == userId);

        if (cartHeaderFromDb is not null)
        {
            Cart cart = new Cart();
            cart.CartHeader = cartHeaderFromDb;
            cart.CartDetails = _dbContext.CartDetails.Where(cd =>
                cd.CartHeaderId == cartHeaderFromDb.CartHeaderId).Include(cd => cd.Product);
            return _mapper.Map<CartDto>(cart);
        }

        return null;
    }

    public async Task<CartDto> CreateUpdateCartAsync(CartDto cartDto)
    {
        Cart cart = _mapper.Map<Cart>(cartDto);

        var productInDb =
            await _dbContext.Products.FirstOrDefaultAsync(p =>
                p.ProductId == cartDto.CartDetails.FirstOrDefault().ProductId);

        if (productInDb is null)
        {
            await _dbContext.Products.AddAsync(cart.CartDetails.FirstOrDefault().Product);
            await _dbContext.SaveChangesAsync();
        }

        var cartHeaderFromDb =
            await _dbContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(ch => ch.UserId == cartDto.CartHeader.UserId);
        if (cartHeaderFromDb is null)
        {
            await _dbContext.CartHeaders.AddAsync(cart.CartHeader);
            await _dbContext.SaveChangesAsync();
            cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
            cart.CartDetails.FirstOrDefault().Product = null;
            await _dbContext.CartDetails.AddAsync(cart.CartDetails.FirstOrDefault());
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            var cartDetailsFromDb = await _dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(cd => cd.ProductId ==
                cart.CartDetails.FirstOrDefault()
                    .ProductId && cd.CartHeaderId == cartHeaderFromDb.CartHeaderId);

            if (cartDetailsFromDb is null)
            {
                cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                await _dbContext.CartDetails.AddAsync(cart.CartDetails.FirstOrDefault());
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                cart.CartDetails.FirstOrDefault().Product = null;
                cart.CartDetails.FirstOrDefault().Count += cartDetailsFromDb.Count;
                cart.CartDetails.FirstOrDefault().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                _dbContext.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                await _dbContext.SaveChangesAsync();
            }
        }

        return _mapper.Map<CartDto>(cart);
    }

    public async Task<bool> RemoveFromCartAsync(int cartDetailsId)
    {
        try
        {
            CartDetails cartDetails = await _dbContext.CartDetails
                .FirstOrDefaultAsync(u => u.CartDetailsId == cartDetailsId);

            int totalCountOfCartItems = _dbContext.CartDetails.Count(u => u.CartHeaderId == cartDetails.CartHeaderId);

            _dbContext.CartDetails.Remove(cartDetails);
            if (totalCountOfCartItems == 1)
            {
                var cartHeaderToRemove = await _dbContext.CartHeaders
                    .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                _dbContext.CartHeaders.Remove(cartHeaderToRemove);
            }
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch(Exception e)
        {
            return false;
        }
    }

    public async Task<bool> ClearCartAsync(string userId)
    {
        var cartHeaderFromDb = await _dbContext.CartHeaders.FirstOrDefaultAsync(ch => ch.UserId == userId);

        if (cartHeaderFromDb is not null)
        {
            _dbContext.CartDetails.RemoveRange(
                _dbContext.CartDetails.Where(cd => cd.CartHeaderId == cartHeaderFromDb.CartHeaderId));

            _dbContext.CartHeaders.Remove(cartHeaderFromDb);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> ApplyCoupon(string userId, string couponCode)
    {
        var cartFromDb = await _dbContext.CartHeaders.FirstOrDefaultAsync(ch => ch.UserId == userId);
        cartFromDb.CouponCode = couponCode;

        _dbContext.CartHeaders.Update(cartFromDb);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveCoupon(string userId)
    {
        var cartFromDb = await _dbContext.CartHeaders.FirstOrDefaultAsync(ch => ch.UserId == userId);
        cartFromDb.CouponCode = string.Empty;

        _dbContext.CartHeaders.Update(cartFromDb);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}