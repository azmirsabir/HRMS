
namespace Application.Contracts.Authentication;

public record RefreshTokenResponse(
    string Token,
    string RefreshToken
);