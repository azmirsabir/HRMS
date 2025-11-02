using Application.Abstractions.Data;
using Application.Contracts;
using Application.Contracts.Authentication;
using Application.Interfaces;
using Core.Domain.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Application.Features.Authorizations.Commands.Login;

public enum LoginCommandPossibleFailures
{
    UserNotFound = 1,
    InvalidData = 2,
    LockedOut = 3,
    NotAllowed = 4,
    RequiresTwoFactor = 5,
    AccountNotConfirmed = 6
}

public record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<AuthenticationResponse, LoginCommandPossibleFailures>>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class
    LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResponse, LoginCommandPossibleFailures>>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IApplicationDbContext _applicationDbContext;

    public LoginCommandHandler(UserManager<User> userManager, SignInManager<User> signInManager,
        ITokenService tokenService, IApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Result<AuthenticationResponse, LoginCommandPossibleFailures>> Handle(LoginCommand request,
        CancellationToken cancellationToken)
    {
        // First find user by email or phone
        var user = await _userManager.FindByEmailAsync(request.Email) ?? _userManager.Users
            .FirstOrDefault(u => u.PhoneNumber == request.Email);

        if (user is null)
        {
            return Result<AuthenticationResponse, LoginCommandPossibleFailures>.Failure(LoginCommandPossibleFailures
                .UserNotFound);
        }

        // Now fetch the complete user with UserBranches and Branch navigation properties
        var foundUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

        if (foundUser is null)
        {
            return Result<AuthenticationResponse, LoginCommandPossibleFailures>.Failure(LoginCommandPossibleFailures
                .UserNotFound);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            foundUser,
            request.Password,
            false
        );

        if (!result.Succeeded)
        {
            return result.ToString() switch
            {
                "LockedOut" => Result<AuthenticationResponse, LoginCommandPossibleFailures>.Failure(
                    LoginCommandPossibleFailures.LockedOut),
                "NotAllowed" => Result<AuthenticationResponse, LoginCommandPossibleFailures>.Failure(
                    LoginCommandPossibleFailures.NotAllowed),
                "RequiresTwoFactor" => Result<AuthenticationResponse, LoginCommandPossibleFailures>.Failure(
                    LoginCommandPossibleFailures.RequiresTwoFactor),
                _ => Result<AuthenticationResponse, LoginCommandPossibleFailures>.Failure(LoginCommandPossibleFailures
                    .InvalidData),
            };
        }

        if (!foundUser.EmailConfirmed)
        {
            return Result<AuthenticationResponse, LoginCommandPossibleFailures>.Failure(LoginCommandPossibleFailures
                .AccountNotConfirmed);
        }

        var response = AuthenticationResponse.ToDto(
            foundUser,
            await _userManager.GetRolesAsync(foundUser).ConfigureAwait(false),
            await _tokenService.GenerateAccessTokenAsync(foundUser.Id),
            await _tokenService.GenerateRefreshTokenAsync(foundUser.Id, cancellationToken)
        );

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return Result<AuthenticationResponse, LoginCommandPossibleFailures>.Success(response);
    }
}