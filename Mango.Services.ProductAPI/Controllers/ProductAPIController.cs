using Mango.Services.ProductAPI.Models.Dtos;
using Mango.Services.ProductAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductAPIController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            var response = new ResponseDto();
            try
            {
                IEnumerable<ProductDto> productDtos = await _productRepository.GetProductsAsync();
                response.Result = productDtos;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return response;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<object> GetById(int id)
        {
            var response = new ResponseDto();
            try
            {
                ProductDto productDto = await _productRepository.GetProductByIdAsync(id);
                response.Result = productDto;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return response;
        }

        [HttpPost]
        public async Task<object> Create([FromBody] ProductDto productDto)
        {
            var response = new ResponseDto();
            try
            {
                ProductDto model = await _productRepository.CreateUpdateProductAsync(productDto);
                response.Result = model;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return response;
        }

        [HttpPut]
        [Authorize]
        public async Task<object> Update([FromBody] ProductDto productDto)
        {
            var response = new ResponseDto();
            try
            {
                ProductDto model = await _productRepository.CreateUpdateProductAsync(productDto);
                response.Result = model;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return response;
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<object> Delete(int id)
        {
            var response = new ResponseDto();
            try
            {
                bool isSuccess = await _productRepository.DeleteProductAsync(id);
                response.Result = isSuccess;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return response;
        }
    }
}
