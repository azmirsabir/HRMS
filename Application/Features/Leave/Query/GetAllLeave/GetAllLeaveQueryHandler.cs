using Application.Abstractions.Data;
using Application.Contracts;
using Application.Features.Leave.Dto.Response;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Leave.Query.GetAllLeave;

public class GetAllLeaveHandler : IRequestHandler<GetAllLeaveQuery,
    Result<PaginatedResponse<LeaveResponse>, GetAllLeavePossibleErrors>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly ICurrentUser _currentUser;

    public GetAllLeaveHandler(IApplicationDbContext applicationDbContext, ICurrentUser currentUser)
    {
        _applicationDbContext = applicationDbContext;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedResponse<LeaveResponse>, GetAllLeavePossibleErrors>> Handle(
        GetAllLeaveQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.Id;
        var roles = _currentUser.Roles;
        
        var query = _applicationDbContext.Leaves
            .Include(l => l.Employee)
            .ThenInclude(e => e.Department)
            .AsQueryable();
        
        
        if (!roles.Contains("Auditor"))
        {
            var user = await _applicationDbContext.Users
                .AsNoTracking()
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

            query = roles.Contains("Employee")
                ? query.Where(l => l.EmployeeId == user.EmployeeId)
                : query.Where(l => l.Employee.DepartmentId == user.Employee.DepartmentId);;

        }

        if (request.EmployeeId is > 0)
        {
            query = query.Where(e => e.EmployeeId == request.EmployeeId);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(e => e.Status == request.Status);
        }
        
        if (request.Type.HasValue)
        {
            query = query.Where(e => e.Type == request.Type);
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            var lowerName = request.FullName.ToLower();
            query = query.Where(l => l.Employee.FullName.ToLower().Contains(lowerName));
        }

        var paginatedResponse = await PaginatedResponse<LeaveResponse>.CreateAsync(
            LeaveResponse.ToResponse(query),
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        return Result<PaginatedResponse<LeaveResponse>, GetAllLeavePossibleErrors>.Success(
            paginatedResponse
        );
    }
}