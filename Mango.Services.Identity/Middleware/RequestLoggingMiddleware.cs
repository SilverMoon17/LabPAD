namespace Mango.Services.Identity.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Получаем реферер
        var referer = context.Request.Headers["Referer"].ToString();
        
        // Если реферер отсутствует, можно использовать другие методы
        if (string.IsNullOrEmpty(referer))
        {
            referer = context.Request.Headers["Origin"].ToString(); // Попробуйте получить Origin
        }

        // Если и Origin отсутствует, можете использовать IP-адрес клиента
        if (string.IsNullOrEmpty(referer))
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            referer = clientIp ?? "Unknown"; // Используем Unknown, если IP не найден
        }

        // Логируем информацию
        _logger.LogInformation($"Request from Referer: {referer}");

        // Вызываем следующий middleware
        await _next(context);
    }
}
