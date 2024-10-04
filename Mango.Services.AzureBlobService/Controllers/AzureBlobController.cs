using Mango.Services.AzureBlobService.Models;
using Mango.Services.AzureBlobService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Mango.Services.AzureBlobService.Controllers;

[ApiController]
[Route("api/AzureBlob")]
public class AzureBlobController : ControllerBase
{
    private readonly IAzureBlobRepository _azureBlobRepository;

    public AzureBlobController(IAzureBlobRepository azureBlobRepository)
    {
        _azureBlobRepository = azureBlobRepository;
    }
    
    [HttpPost("Upload")]
    [Authorize]
    public async Task<ResponseDto> Upload(IFormFile file)
    {
        var response = new ResponseDto();
        
        if (file == null || file.Length == 0)
        {
            response.IsSuccess = false;
            response.DisplayMessage = "File is undefined";
            return response;
        }
        
        try
        {
            string url = await _azureBlobRepository.UploadImageAsync(file);
            response.Result = url;
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return response;
    }
    
    [HttpDelete("Delete")]
    [Authorize]
    public async Task<ResponseDto> Delete([FromBody]string imageUrl)
    {
        var response = new ResponseDto();

        if (imageUrl.IsNullOrEmpty())
        {
            response.IsSuccess = false;
            response.DisplayMessage = "File Name is null or empty";
        }
        
        try
        {
            bool isSuccess = await _azureBlobRepository.DeleteImageAsync(imageUrl);
            response.IsSuccess = isSuccess;
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return response;
    }
}