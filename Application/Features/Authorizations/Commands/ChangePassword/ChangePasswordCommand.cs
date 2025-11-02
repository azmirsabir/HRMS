using Application.Abstractions.Data;
using Application.Contracts;
using Application.Interfaces;
using Core.Domain.Authentications;
using Core.Domain.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Authorizations.Commands.ChangePassword;

public enum ChangePasswordCommandPossibleFailures
{
    UserNotFound = 1,
    UnableToChange = 2,
    PasswordMismatch = 3,
    InvalidPassword = 4,
    SamePassword = 5, 
    UserIsLockedOut = 6,
    UserNotConfirmed = 7
}

public record ChangePasswordCommand(
    string UserId,
    string Token,
    string CurrentPassword,
    string Password,
    string ConfirmPassword
) : IRequest<Result<string, ChangePasswordCommandPossibleFailures>>;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CurrentPassword).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Password).NotEmpty()
            .MinimumLength(8)
            .Must(password => password.Any(char.IsUpper))
            .Must(password => password.Any(char.IsLower))
            .Must(password => password.Any(char.IsDigit))
            .Must(password => !password.Any(char.IsWhiteSpace));
        RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.Password);
    }
}

partial class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<string, ChangePasswordCommandPossibleFailures>>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IApplicationDbContext _applicationDbContext;

    public ChangePasswordCommandHandler(UserManager<User> userManager, ITokenService tokenService, IApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Result<string, ChangePasswordCommandPossibleFailures>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if(user is null)
        {
            return Result<string, ChangePasswordCommandPossibleFailures>.Failure(ChangePasswordCommandPossibleFailures.UserNotFound);
        }
        
        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.Password);
        if (!result.Succeeded)
        {
            var errorCodes = result.Errors.Select(e => e.Code).ToList();
            return errorCodes.First() switch
            {
                "PasswordMismatch" => Result<string, ChangePasswordCommandPossibleFailures>.Failure(ChangePasswordCommandPossibleFailures.PasswordMismatch),
                "InvalidPassword" => Result<string, ChangePasswordCommandPossibleFailures>.Failure(ChangePasswordCommandPossibleFailures.InvalidPassword),
                "DuplicatePassword" => Result<string, ChangePasswordCommandPossibleFailures>.Failure(ChangePasswordCommandPossibleFailures.SamePassword),
                "UserIsLockedOut" => Result<string, ChangePasswordCommandPossibleFailures>.Failure(ChangePasswordCommandPossibleFailures.UserIsLockedOut),
                "UserNotConfirmed" => Result<string, ChangePasswordCommandPossibleFailures>.Failure(ChangePasswordCommandPossibleFailures.UserNotConfirmed),
                _ => Result<string, ChangePasswordCommandPossibleFailures>.Failure(ChangePasswordCommandPossibleFailures.UnableToChange),
            };
        }
        await _userManager.UpdateSecurityStampAsync(user);
        
        var invalidToken = new InvalidTokens(request.Token);
        await _tokenService.InvalidateToken(invalidToken, cancellationToken);
        await _tokenService.RevokeToken(user.Id, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return Result<string, ChangePasswordCommandPossibleFailures>.Success("password changed successfully");
    }
}