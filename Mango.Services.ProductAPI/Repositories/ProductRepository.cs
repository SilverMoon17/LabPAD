using AutoMapper;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ProductRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        List<Product> productList = await _dbContext.Products.ToListAsync();
        return _mapper.Map<List<ProductDto>>(productList);
    }

    public async Task<ProductDto> GetProductByIdAsync(int productId)
    {
        Product product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateUpdateProductAsync(ProductDto productDto)
    {
        Product product = _mapper.Map<Product>(productDto);

        if (product.ProductId > 0)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }

        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        try
        {
            Product product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product is null) return false;
            
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}