using Mango.Web.Models;
using Mango.Web.Models.ProductModels;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IAzureBlobService _azureBlobService;
        public ProductController(IProductService productService, IAzureBlobService azureBlobService)
        {
            _productService = productService;
            _azureBlobService = azureBlobService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto> list = new();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetAllProductsAsync<ResponseDto>(accessToken);
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }
        
        [Authorize]
        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }
        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate(ProductCreateDto model)
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var azureBlobResponse = await _azureBlobService.UploadImage<ResponseDto>(model.Image, accessToken);
                if (azureBlobResponse is not null && azureBlobResponse.IsSuccess)
                {
                    var productDto = new ProductDto()
                    {
                        ProductId = model.ProductId,
                        Name = model.Name,
                        Price = model.Price,
                        Description = model.Description,
                        CategoryName = model.CategoryName,
                        ImageUrl = azureBlobResponse.Result.ToString(),
                        Count = model.Count
                    };
                    var response = await _productService.CreateProductAsync<ResponseDto>(productDto, accessToken);
                    if (response != null && response.IsSuccess)
                    {
                        return RedirectToAction(nameof(ProductIndex));
                    }
                }
            }
            return View(model);
        }
        public async Task<IActionResult> ProductEdit(int productId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);
            if (response != null && response.IsSuccess)
            {
                ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _productService.UpdateProductAsync<ResponseDto>(model, accessToken);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> ProductDelete(int productId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);
            if (response != null && response.IsSuccess)
            {
                ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var imageDeleted = await _azureBlobService.DeleteImage<ResponseDto>(model.ImageUrl, accessToken);
                if (imageDeleted.IsSuccess)
                {
                    var response = await _productService.DeleteProductAsync<ResponseDto>(model.ProductId, accessToken);
                    if (response.IsSuccess)
                    {
                        return RedirectToAction(nameof(ProductIndex));
                    }
                }
            }
            return View(model);
        }
    }
}