using System.Net.Http.Headers;
using System.Text;
using Mango.Web.Models;
using Newtonsoft.Json;

namespace Mango.Web.Services.IServices;

public class BaseService : IBaseService
{
    public ResponseDto responseModel { get; set; }
    public IHttpClientFactory httpClient { get; set; }
    
    public BaseService(IHttpClientFactory httpClient)
    {
        this.responseModel = new ResponseDto();
        this.httpClient = httpClient;
    }
    
    public async Task<T> SendAsync<T>(ApiRequest apiRequest)
{
    try
    {
        var client = httpClient.CreateClient("MangoAPI");
        HttpRequestMessage message = new HttpRequestMessage
        {
            RequestUri = new Uri(apiRequest.Url)
        };
        message.Headers.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Clear();

        if (!string.IsNullOrEmpty(apiRequest.AccessToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.AccessToken);
        }

        if (apiRequest.Data != null)
        {
            if (apiRequest.Data is IFormFile || apiRequest.Data is List<IFormFile>)
            {
                // Handle file upload
                var multipartContent = new MultipartFormDataContent();
                if (apiRequest.Data is IFormFile file)
                {
                    multipartContent.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);
                }
                else if (apiRequest.Data is List<IFormFile> files)
                {
                    foreach (var f in files)
                    {
                        multipartContent.Add(new StreamContent(f.OpenReadStream()), "files", f.FileName);
                    }
                }
                message.Content = multipartContent;
            }
            else
            {
                // Handle normal JSON data
                message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
            }
        }

        HttpResponseMessage apiResponse = null;
        switch (apiRequest.ApiType)
        {
            case SD.ApiType.POST:
                message.Method = HttpMethod.Post;
                break;
            case SD.ApiType.PUT:
                message.Method = HttpMethod.Put;
                break;
            case SD.ApiType.DELETE:
                message.Method = HttpMethod.Delete;
                break;
            default:
                message.Method = HttpMethod.Get;
                break;
        }

        apiResponse = await client.SendAsync(message);

        var apiContent = await apiResponse.Content.ReadAsStringAsync();
        var apiResponseDto = JsonConvert.DeserializeObject<T>(apiContent);

        return apiResponseDto;
    }
    catch (Exception e)
    {
        var dto = new ResponseDto()
        {
            DisplayMessage = "Error",
            ErrorMessages = new List<string> { e.Message },
            IsSuccess = false
        };

        var res = JsonConvert.SerializeObject(dto);
        var apiResponseDto = JsonConvert.DeserializeObject<T>(res);

        return apiResponseDto;
    }
}

    
    public void Dispose()
    {
        GC.SuppressFinalize(true);
    }
}