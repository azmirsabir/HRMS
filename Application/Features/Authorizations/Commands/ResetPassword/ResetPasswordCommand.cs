using System.Text;
using Application.Contracts;
using Core.Domain.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Features.Authorizations.Commands.ResetPassword;

public enum ResetPasswordCommandPossibleFailures
{
    UserNotFound = 1,
    InvalidToken = 2,
    InvalidPassword = 3,
    FailedResetPassword = 4,
    UnknownError = 5
}


public record ResetPasswordCommand(
    string? Email,
    string Token,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<Result<string, ResetPasswordCommandPossibleFailures>>;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        
        RuleFor(x => x.NewPassword).NotEmpty()
            .MinimumLength(8)
            .Must(password => password.Any(char.IsUpper))
            .Must(password => password.Any(char.IsLower))
            .Must(password => password.Any(char.IsDigit))
            .Must(password => !password.Any(char.IsWhiteSpace));
        RuleFor(x => x.ConfirmNewPassword).NotEmpty().Equal(x => x.NewPassword);
    }
}

partial class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<string, ResetPasswordCommandPossibleFailures>>
{
    private readonly UserManager<User> _userManager;
    private readonly IMemoryCache _cache;

    public ResetPasswordCommandHandler(UserManager<User> userManager, IMemoryCache cache)
    {
        _userManager = userManager;
        _cache = cache;
    }

    public async Task<Result<string, ResetPasswordCommandPossibleFailures>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user =  _userManager.Users.FirstOrDefault(u=> u.Email == request.Email);

        if (user is null)
        {
            return Result<string, ResetPasswordCommandPossibleFailures>.Failure(ResetPasswordCommandPossibleFailures.UserNotFound);
        }

        if (request.Email!=null)
       
        {
            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));


            var result = await _userManager.ResetPasswordAsync(user, decodedCode, request.NewPassword);

            if (!result.Succeeded)
            {
                var errorCodes = result.Errors.Select(e => e.Code).ToList();

                return errorCodes.First() switch
                {
                    "InvalidToken" => Result<string, ResetPasswordCommandPossibleFailures>.Failure(
                        ResetPasswordCommandPossibleFailures.InvalidToken),
                    "InvalidPassword" => Result<string, ResetPasswordCommandPossibleFailures>.Failure(
                        ResetPasswordCommandPossibleFailures.InvalidPassword),
                    _ => Result<string, ResetPasswordCommandPossibleFailures>.Failure(
                        ResetPasswordCommandPossibleFailures.FailedResetPassword),
                };
            }

            return Result<string, ResetPasswordCommandPossibleFailures>.Success("Password changed successfully");
        }
        else
        {

            var savedToken = _cache.Get("ForgetPasswordToken" + user.Id);
            if (savedToken == null)
            {
                return Result<string, ResetPasswordCommandPossibleFailures>.Failure(ResetPasswordCommandPossibleFailures.InvalidToken);
            }

            ;
            if (savedToken.ToString() != request.Token)
            {
                return Result<string, ResetPasswordCommandPossibleFailures>.Failure(ResetPasswordCommandPossibleFailures.InvalidToken);
            }

            try
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.NewPassword);
                var result = _userManager.Users.ExecuteUpdate(
                    u => u.SetProperty(x => x.PasswordHash, user.PasswordHash)
                );
            }
            catch (Exception e)
            {
                return Result<string, ResetPasswordCommandPossibleFailures>.Failure(ResetPasswordCommandPossibleFailures.UnknownError);
            }  
         

            _cache.Remove("ForgetPasswordToken" + user.Id);

            return Result<string, ResetPasswordCommandPossibleFailures>.Success("Password changed successfully");
        }
        
    }
}