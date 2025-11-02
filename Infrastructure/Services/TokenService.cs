using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Core.Domain.Authentications;
using Core.Domain.Users;
using Infrastructure.Configurations;
using Infrastructure.Database.Context;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class TokenService(
    UserManager<User> userManager,
    IOptions<JwtSetting> jwtSettings,
    MainContext context) : ITokenService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly JwtSetting _jwtSettings = jwtSettings.Value;
    private readonly MainContext _context = context;
   
   public async Task<string> GenerateAccessTokenAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId) ??
        throw new InfrastructureException("the user is not founded", StatusCodes.Status404NotFound);

        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();
        for (int i = 0; i < roles.Count; i++)
        {
            roleClaims.Add(new Claim("roles", roles[i]));
        }


        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim("uid", user.Id)
        }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

    public async Task<string> GenerateRefreshTokenAsync(string userId, CancellationToken cancellationToken)
    {
        var refreshToken = new RefreshToken(userId, _jwtSettings.RefreshTokenExpirationDays);
        await _context.Set<RefreshToken>().AddAsync(refreshToken, cancellationToken);
        return refreshToken.Token;
    }


    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Key))
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var validatedToken);
        }
        catch
        {
            throw new InfrastructureException("invalid token principle", StatusCodes.Status404NotFound);
        }
    }

    public async Task RevokeToken(string userId, CancellationToken cancellationToken)
    {
        var refreshTokens = await _context.Set<RefreshToken>()
        .Where(x => x.UserId == userId)
        .ToListAsync(cancellationToken);

        foreach (var refreshToken in refreshTokens)
        {
            refreshToken.IsRevoked = true;
        }

        _context.Set<RefreshToken>().UpdateRange(refreshTokens);
    }

    public async Task<bool> IsTokenBlacklisted(string token)
    {
        try
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtTokenHandler.ReadJwtToken(token);

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (jti == null) return false;

            return await _context.Set<InvalidTokens>().AnyAsync(bt => bt.Jti == jti);
        }
        catch (Microsoft.IdentityModel.Tokens.SecurityTokenMalformedException)
        {
            // Token is malformed/invalid, so it's not blacklisted (it's just invalid)
            return false;
        }
        catch (Microsoft.IdentityModel.Tokens.SecurityTokenException)
        {
            // Any other security token exception means the token is invalid, not blacklisted
            return false;
        }
    }

    public async Task InvalidateToken(InvalidTokens token, CancellationToken cancellationToken)
    {
        await _context.Set<InvalidTokens>().AddAsync(token, cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        return await _context.Set<RefreshToken>()
        .Where(x => x.Token == refreshToken)
        .FirstOrDefaultAsync(cancellationToken);
    }
}