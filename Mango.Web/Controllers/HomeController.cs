using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mango.Web.Models;
using Mango.Web.Models.CartModels;
using Mango.Web.Models.ProductModels;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly ICartService _cartService;

    public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
    {
        _logger = logger;
        _productService = productService;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        List<ProductDto> list = new List<ProductDto>();
        
        var response = await _productService.GetAllProductsAsync<ResponseDto>(accessToken);

        if (response is not null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
        }
        
        return View(list);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    [Authorize]
    public async Task<IActionResult> Login()
    {
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Logout()
    {
        return SignOut("Cookies", "oidc");
    }

    [Authorize]
    public async Task<IActionResult> Details(int productId)
    {
        var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, "");

        if (response is not null && response.IsSuccess)
        {
            ProductDto productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            return View(productDto);
        }

        return NotFound();
    }
    
    [Authorize]
    [HttpPost]
    [ActionName("Details")]
    public async Task<IActionResult> DetailsPost(ProductDto productDto)
    {
        CartDto cartDto = new CartDto
        {
            CartHeader = new CartHeaderDto
            {
                UserId = User.Claims.Where(u => u.Type=="sub")?.FirstOrDefault()?.Value
            }
        };

        CartDetailsDto cartDetailsDto = new CartDetailsDto
        {
            ProductId = productDto.ProductId,
            Count = productDto.Count
        };

        var resp = await _productService.GetProductByIdAsync<ResponseDto>(productDto.ProductId, "");

        if (resp is not null && resp.IsSuccess)
        {
            cartDetailsDto.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(resp.Result));
        }

        List<CartDetailsDto> cartDetailsDtos = new List<CartDetailsDto>();
        cartDetailsDtos.Add(cartDetailsDto);
        cartDto.CartDetails = cartDetailsDtos;

        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var addToCartResp = await _cartService.AddToCartAsync<ResponseDto>(cartDto, accessToken);

        if (addToCartResp is not null && addToCartResp.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }
        
        return View(productDto);
    }
}