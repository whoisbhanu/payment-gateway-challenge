using System.Net;
using System.Net.Mime;
using System.Text.Json;

using PaymentGateway.Clients;

namespace PaymentGateway.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ExternalHttpRequestException ex)
        {
            await HandleExternalExceptionAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        _logger.LogError("Error occurred {Error}", exception);
        await context.Response.WriteAsync($"Something went wrong. Status Code : {context.Response.StatusCode}");
    }
    
    private async Task HandleExternalExceptionAsync(HttpContext context, ExternalHttpRequestException exception)
    {
        if (exception.StatusCode == HttpStatusCode.BadRequest)
        {
            var result = JsonSerializer.Serialize(
                new { title = "Not supported by bank", status = exception.StatusCode });

            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            _logger.LogError("Error occurred - {Error}", exception);
            await context.Response.WriteAsync(result);
        }
        // handle other scenarios
    }
}