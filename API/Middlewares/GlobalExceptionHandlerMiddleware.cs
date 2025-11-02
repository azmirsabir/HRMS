using System.Net;
using Core.Domain;
using Infrastructure.Exceptions;

namespace API.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch(InfrastructureException ex)
        {
            _logger.LogError(ex, ex.Message);
            httpContext.Response.StatusCode = ex.StatusCode;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, ex.Message);
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, ex.Message);
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new { message = "Unauthorized" });
        }
        catch (Microsoft.IdentityModel.Tokens.SecurityTokenMalformedException ex)
        {
            _logger.LogWarning(ex, "Malformed JWT token received: {Message}", ex.Message);
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new { message = "Invalid token format" });
        }
        catch (Microsoft.IdentityModel.Tokens.SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "Security token exception: {Message}", ex.Message);
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new { message = "Invalid token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
    }
}