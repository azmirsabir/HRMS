using Application.Contracts;
using Application.Contracts.Authentication;
using Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Features.Authorizations.Commands.GenerateRefreshToken;

public enum RefreshTokenCommandPossibleFailures
{
    NotFound = 1,
    NotValid = 2
}

public record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken
) : IRequest<Result<RefreshTokenResponse, RefreshTokenCommandPossibleFailures>>;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

partial class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse, RefreshTokenCommandPossibleFailures>>
{
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<Result<RefreshTokenResponse, RefreshTokenCommandPossibleFailures>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _tokenService.GetRefreshToken(request.RefreshToken, cancellationToken);
        if(refreshToken is null)
        {
            return Result<RefreshTokenResponse, RefreshTokenCommandPossibleFailures>.Failure(RefreshTokenCommandPossibleFailures.NotFound);
        }

        if(!refreshToken.IsValidRefreshToken())
        {
            return Result<RefreshTokenResponse, RefreshTokenCommandPossibleFailures>.Failure(RefreshTokenCommandPossibleFailures.NotValid);
        }

        var accessToken = await _tokenService.GenerateAccessTokenAsync(refreshToken.UserId);

        var response = new RefreshTokenResponse(accessToken, refreshToken.Token);
        return Result<RefreshTokenResponse, RefreshTokenCommandPossibleFailures>.Success(response);
    }
}