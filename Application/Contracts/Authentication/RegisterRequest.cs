
namespace Application.Contracts.Authentication;

public record RegisterRequest(
    string FullName,
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword,
    string Phone,
    string Role,
    int EmployeeId
    );