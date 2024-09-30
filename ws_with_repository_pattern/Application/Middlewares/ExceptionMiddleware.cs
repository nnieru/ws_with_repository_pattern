using System.Net;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using ws_with_repository_pattern.Application.Exception;
using ws_with_repository_pattern.Response;

namespace ws_with_repository_pattern.Application.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        try
        {
            Console.WriteLine(context.Response);
            await _next(context);
        }
        catch (System.Exception e)
        {
            var response = context.Response;
            response.ContentType = "application/json";
       
            var (status, message) = HandleErrorResponse(e);
            response.StatusCode = (int) status;
            var mappedResponse = new BaseResponse<object>
            {
                StatusCode = status,
                message = message
            };
            
            _logger.LogError($"{context.Request.Path} - ERROR {message}");
            
            await response.WriteAsync(JsonSerializer.Serialize(mappedResponse));
        }
    }

    private (HttpStatusCode code, string message) HandleErrorResponse(System.Exception exception)
    {
        HttpStatusCode code;
        string message = exception.Message;
        switch (exception)
        {
            case KeyNotFoundException or
                FileNotFoundException or
                UserNotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            case UnauthorizedAccessException or 
                SecurityTokenExpiredException:
                code = HttpStatusCode.Unauthorized;
                break;
            case ArgumentException or 
                InvalidOperationException:
                code = HttpStatusCode.BadRequest;
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                message = "System encounter some error, please try again later";
                break;
        }

        return (code, message);
    }
}



public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}