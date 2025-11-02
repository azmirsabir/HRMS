using Application.Interfaces;

namespace API.Middlewares;

public class TokenInValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenInValidationMiddleware> _logger;

    public TokenInValidationMiddleware(RequestDelegate next, ILogger<TokenInValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using (var scope = context.RequestServices.CreateScope())
        {
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
            var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            if (token != null && await tokenService.IsTokenBlacklisted(token))
            {
                _logger.LogWarning("Attempt to use a blacklisted token: {Token}", token);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token has been revoked.");
                return;
            }
        }
        await _next(context);
    }
}