using Core.Domain.Users;

namespace Application.Contracts.Authentication;

public record AuthenticationResponse(
    string UserId,
    string Email,
    string FullName,
    string UserName,
    IEnumerable<string> Roles,
    string Token,
    string RefreshToken)
{
    public static AuthenticationResponse ToDto(User user, IEnumerable<string> roles, string token,
        string refreshToken)
    {
        return new AuthenticationResponse(
            user.Id,
            user.Email!,
            user.FullName,
            user.UserName!,
            roles,
            token,
            refreshToken
        );
    }
}