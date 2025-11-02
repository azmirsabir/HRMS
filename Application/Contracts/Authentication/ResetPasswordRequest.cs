
namespace Application.Contracts.Authentication;

public record ResetPasswordRequest(
    string? Email, 
    string Token,
    string NewPassword,
    string ConfirmNewPassword
);