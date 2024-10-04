using Mango.Services.ProductAPI.Models.Dtos;

namespace Mango.Services.ProductAPI.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<ProductDto>> GetProductsAsync();
    Task<ProductDto> GetProductByIdAsync(int productId);
    Task<ProductDto> CreateUpdateProductAsync(ProductDto productDto);
    Task<bool> DeleteProductAsync(int productId);
}