using Application.Abstractions.Data;
using Application.Contracts;
using Application.Interfaces;
using Core.Domain.Authentications;
using MediatR;

namespace Application.Features.Authorizations.Commands.Logout;

public enum LogoutCommandPossibleFailures
{
    
}

public record LogoutCommand(string Token) : IRequest<Result<string, LogoutCommandPossibleFailures>>;

partial class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<string, LogoutCommandPossibleFailures>>
{
    private readonly ITokenService _tokenService;
    private readonly IApplicationDbContext _applicationDbContext;

    public LogoutCommandHandler(ITokenService tokenService, IApplicationDbContext applicationDbContext)
    {
        _tokenService = tokenService;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Result<string, LogoutCommandPossibleFailures>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var inValidToken = new InvalidTokens(request.Token);
        await _tokenService.InvalidateToken(inValidToken, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return Result<string, LogoutCommandPossibleFailures>.Success("successfully logged out");
    }
}