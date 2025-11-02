using Application.Abstractions.Data;
using Application.Contracts;
using Application.Interfaces;
using Core.Domain.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Authorizations.Commands.Register;

public enum RegisterCommandPossibleFailures
{
    DuplicateUserName = 1,
    DuplicateEmail = 2,
    InvalidUserName = 3,
    InvalidEmail = 4,
    InvalidPassword = 5,
    UserLockedOut = 6,
    UnableToCreateUser = 7,
    InvalidRole = 8
}

public record RegisterCommand(
    string FullName,
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword,
    string Phone,
    string Role,
    int EmployeeId) : IRequest<Result<string, RegisterCommandPossibleFailures>>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(2);
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(2);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty()
            .MinimumLength(8);
        RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.Password);
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\d+$").MaximumLength(16);
        RuleFor(x => x.Role).NotEmpty();
        RuleFor(x => x.EmployeeId).GreaterThan(0);
    }
}

partial class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string, RegisterCommandPossibleFailures>>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ICurrentUser _currentUser;
    private readonly IApplicationDbContext _context;


    public RegisterCommandHandler(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
        ICurrentUser currentUser, IApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _currentUser = currentUser;
        _context = context;
    }

    public async Task<Result<string, RegisterCommandPossibleFailures>> Handle(RegisterCommand request,
        CancellationToken cancellationToken)
    {
        if (!await _roleManager.RoleExistsAsync(request.Role))
        {
            return Result<string, RegisterCommandPossibleFailures>.Failure(RegisterCommandPossibleFailures.InvalidRole);
        }

        var user = new User(
            request.FullName,
            request.UserName,
            request.EmployeeId,
            request.Email,
            request.Phone,
            true
            // _currentUser.Roles.Contains("Manager")?true:false
        );

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errorCodes = result.Errors.Select(e => e.Code).ToList();
            return errorCodes.First() switch
            {
                "DuplicateUserName" => Result<string, RegisterCommandPossibleFailures>.Failure(
                    RegisterCommandPossibleFailures.DuplicateUserName),
                "DuplicateEmail" => Result<string, RegisterCommandPossibleFailures>.Failure(
                    RegisterCommandPossibleFailures.DuplicateEmail),
                "InvalidUserName" => Result<string, RegisterCommandPossibleFailures>.Failure(
                    RegisterCommandPossibleFailures.InvalidUserName),
                "InvalidEmail" => Result<string, RegisterCommandPossibleFailures>.Failure(
                    RegisterCommandPossibleFailures.InvalidEmail),
                "InvalidPassword" => Result<string, RegisterCommandPossibleFailures>.Failure(
                    RegisterCommandPossibleFailures.InvalidPassword),
                "UserLockedOut" => Result<string, RegisterCommandPossibleFailures>.Failure(
                    RegisterCommandPossibleFailures.UserLockedOut),
                _ => Result<string, RegisterCommandPossibleFailures>.Failure(RegisterCommandPossibleFailures
                    .UnableToCreateUser),
            };
        }

        await _userManager.AddToRoleAsync(user, request.Role);

        return Result<string, RegisterCommandPossibleFailures>.Success("account registered successfully");
    }
}