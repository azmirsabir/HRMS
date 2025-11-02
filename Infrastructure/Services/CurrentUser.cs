using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");
    
    public List<string> Roles => _httpContextAccessor.HttpContext?.User
        .FindAll(c => c.Type == ClaimTypes.Role )
        .Select(c => c.Value)
        .ToList() ?? new List<string>();
}