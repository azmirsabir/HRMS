using System.IdentityModel.Tokens.Jwt;

namespace Core.Domain.Authentications;

public class InvalidTokens
{
    public long Id { get; set; }
    public string Jti { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;

    public InvalidTokens(string jti, string userId)
    {
        Jti = jti;
        UserId = userId;
    }

    public InvalidTokens(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ??
                     throw new DomainException("this token has invalid data");

        this.Jti = jwtToken.Id;
        this.UserId = userId;
    }
}