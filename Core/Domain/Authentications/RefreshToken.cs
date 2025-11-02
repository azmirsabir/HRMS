using System.Security.Cryptography;

namespace Core.Domain.Authentications;

public class RefreshToken
{
    public long Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime ExpireDate { get; set; }
    public bool IsRevoked { get; set; }

    private RefreshToken() { }

    public RefreshToken(string userId, int expireInDays)
    {
        UserId = userId;
        Token = GenerateRandomToken();
        ExpireDate = DateTime.UtcNow.AddDays(expireInDays);
        IsRevoked = false;
    }

    public bool IsValidRefreshToken()
    {
        if(this.ExpireDate < DateTime.UtcNow)
        {
            return false;
        }
        if(this.IsRevoked)
        {
            return false;
        }
        return true;
    }

    private string GenerateRandomToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[40];
        rng.GetBytes(randomBytes);
        return BitConverter.ToString(randomBytes).Replace("-", "").ToLowerInvariant();
    }
}