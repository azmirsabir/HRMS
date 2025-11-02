using System.Security.Claims;
using Core.Domain.Authentications;

namespace Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(string userId);
    Task<string> GenerateRefreshTokenAsync(string userId, CancellationToken cancellationToken);
    Task RevokeToken(string userId, CancellationToken cancellationToken);
    Task<bool> IsTokenBlacklisted(string token);
    Task InvalidateToken(InvalidTokens token, CancellationToken cancellationToken);
    Task<RefreshToken?> GetRefreshToken(string refreshToken, CancellationToken cancellationToken);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}