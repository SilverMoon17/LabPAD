namespace Mango.Services.AzureBlobService.Repositories;

public interface IAzureBlobRepository
{
    public Task<string> UploadImageAsync(IFormFile image);
    public Task<bool> DeleteImageAsync(string fileName);
}