using Application.Contracts.Authentication;
using Application.Features.Authorizations.Commands.ChangePassword;
using Application.Features.Authorizations.Commands.Login;
using Application.Features.Authorizations.Commands.Logout;
using Application.Features.Authorizations.Commands.Register;
using Application.Features.Authorizations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = Application.Contracts.Authentication.LoginRequest;
using RegisterRequest = Application.Contracts.Authentication.RegisterRequest;

namespace API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthorizationsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost("sign-in")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponse>> SignIn(LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new LoginCommand(
            request.Email,
            request.Password
        ), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Data)
            : result.Error switch
            {
                LoginCommandPossibleFailures.UserNotFound => NotFound("User not found."),
                LoginCommandPossibleFailures.LockedOut => Forbid("Account is locked."),
                LoginCommandPossibleFailures.NotAllowed => Forbid("Login is not allowed for this account."),
                LoginCommandPossibleFailures.RequiresTwoFactor => BadRequest("Two-factor authentication is required."),
                LoginCommandPossibleFailures.AccountNotConfirmed => BadRequest("Account is not confirmed."),
                LoginCommandPossibleFailures.InvalidData => BadRequest("Invalid email or password."),
                _ => BadRequest(result.Errors)
            };
    }

    [HttpPost("sign-up")]
    [AllowAnonymous]
    public async Task<ActionResult> SignUp(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RegisterCommand(
            request.FullName,
            request.UserName,
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.Phone,
            request.Role,
            request.EmployeeId), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Data)
            : result.Error switch
            {
                RegisterCommandPossibleFailures.DuplicateUserName => Conflict("Username already exists."),
                RegisterCommandPossibleFailures.DuplicateEmail => Conflict("Email is already registered."),
                RegisterCommandPossibleFailures.InvalidUserName => BadRequest("Username is invalid."),
                RegisterCommandPossibleFailures.InvalidEmail => BadRequest("Email format is invalid."),
                RegisterCommandPossibleFailures.InvalidPassword => BadRequest(
                    "Password must be at least 8 characters long."),
                RegisterCommandPossibleFailures.UserLockedOut => Forbid("User account is locked."),
                _ => BadRequest(result.Errors)
            };
    }

    [HttpPatch("{id}/change-password")]
    public async Task<ActionResult> ChangePassword(string id, ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var accessToken = Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
        if (accessToken is null)
        {
            return Unauthorized("invalid token");
        }

        var result = await _sender.Send(new ChangePasswordCommand(
            id,
            accessToken,
            request.CurrentPassword,
            request.NewPassword,
            request.ConfirmNewPassword
        ), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Data)
            : result.Error switch
            {
                ChangePasswordCommandPossibleFailures.UserNotFound => NotFound("user not Found"),
                ChangePasswordCommandPossibleFailures.UnableToChange => StatusCode(500,
                    "couldn't change the password due to an internal error."),
                ChangePasswordCommandPossibleFailures.PasswordMismatch => BadRequest("wrong password provided"),
                ChangePasswordCommandPossibleFailures.InvalidPassword => BadRequest(
                    "password must at least 8 character"),
                ChangePasswordCommandPossibleFailures.SamePassword => Conflict("the new password is the same"),
                ChangePasswordCommandPossibleFailures.UserIsLockedOut => Forbid("account is locked"),
                ChangePasswordCommandPossibleFailures.UserNotConfirmed => Forbid("account is not confirmed"),
                _ => BadRequest(result.Errors)
            };
    }

    [HttpPost("sign-out")]
    public async Task<ActionResult> SignOut(CancellationToken cancellationToken)
    {
        var accessToken = Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
        if (accessToken is null)
        {
            return Unauthorized("invalid token");
        }

        var result = await _sender.Send(new LogoutCommand(accessToken!), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Data)
            : result.Error switch
            {
                _ => BadRequest(result.Errors)
            };
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> Me()
    {
        var _result = await _sender.Send(new MeQuery(), CancellationToken.None);
        return _result.IsSuccess ? Ok(_result) : BadRequest(_result.Errors);
    }
}