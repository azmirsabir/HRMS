using Application.Contracts;
using Application.Contracts.Authentication;
using Application.Interfaces;
using Core.Domain.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Authorizations.Queries;

public enum MeQueryPossibleFailures
{
    NotFound = 1
}

public record  MeQuery(

) : IRequest<Result<MeResponse,  MeQueryPossibleFailures>>;

public class GetRoleClaimsQueryValidator : AbstractValidator<MeQuery>
{
    public GetRoleClaimsQueryValidator()
    {
    }
}

partial class  MeQueryHandler : IRequestHandler< MeQuery, Result<MeResponse,  MeQueryPossibleFailures>>
{
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUser _currentUser;

    public MeQueryHandler(UserManager<User> userManager, ICurrentUser currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<Result<MeResponse,  MeQueryPossibleFailures>> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        var _userId = _currentUser.Id;
        var user = await _userManager.FindByIdAsync(_userId);
        if(user == null)
        {
            return Result<MeResponse,MeQueryPossibleFailures>.Failure(MeQueryPossibleFailures.NotFound);
        }
        return Result<MeResponse, MeQueryPossibleFailures>.Success(MeResponse.ToResponse(user));
    }
}