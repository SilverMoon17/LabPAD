using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AzureBlobService.Repositories;

public class AzureBlobRepository : IAzureBlobRepository
{
    private readonly string _connectionString;
    private readonly string _containerName;

    public AzureBlobRepository(string connectionString, string containerName)
    {
        _connectionString = connectionString;
        _containerName = containerName;
    }

    public async Task<string> UploadImageAsync(IFormFile image)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);

        // Get the Blob container client
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        // Ensure the container exists
        await containerClient.CreateIfNotExistsAsync();

        // Get the Blob client
        var blobClient = containerClient.GetBlobClient(image.FileName);

        // Upload the file
        using (var stream = image.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, true);
        }

        // Return the URI of the uploaded blob
        return blobClient.Uri.ToString();
    }
    
    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);

        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        var fileName = GetBlobNameFromUrl(imageUrl);

        var blobClient = containerClient.GetBlobClient(fileName);

        var response = await blobClient.DeleteIfExistsAsync();

        return response;
    }
    
    private string GetBlobNameFromUrl(string imageUrl)
    {
        var uri = new Uri(imageUrl);
        var segments = uri.AbsolutePath.TrimStart('/').Split("/");
        return segments.Last();
    }
}