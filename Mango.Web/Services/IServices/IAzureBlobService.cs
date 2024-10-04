namespace Mango.Web.Services.IServices;

public interface IAzureBlobService
{
    Task<T> UploadImage<T>(IFormFile image, string accessToken);
    Task<T> DeleteImage<T>(string imageUrl, string accessToken);
}