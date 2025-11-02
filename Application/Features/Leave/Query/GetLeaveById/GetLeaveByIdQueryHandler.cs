using Application.Abstractions.Data;
using Application.Contracts;
using Application.Features.Leave.Dto.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Leave.Query.GetLeaveById;

public class GetLeaveByIdQueryHandler : IRequestHandler<GetLeaveByIdQuery, Result<LeaveResponse, GetLeaveByIdPossibleErrors>>
{
    private readonly IApplicationDbContext _context;

    public GetLeaveByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<LeaveResponse, GetLeaveByIdPossibleErrors>> Handle(
        GetLeaveByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var leave = await _context.Set<Core.Domain.Leave.Leave>()
            .Include(e => e.Employee)
            .FirstOrDefaultAsync(e => e.Id == request.LeaveId, cancellationToken);

        if (leave == null)
        {
            return Result<LeaveResponse, GetLeaveByIdPossibleErrors>
                .Failure(GetLeaveByIdPossibleErrors.LeaveNotFound);
        }

        var leaveDto = new LeaveResponse
        {
            Id = leave.Id,
            EmployeeName = leave.Employee?.FullName ?? string.Empty,
            DepartmentName = leave.Employee?.Department?.Name ?? string.Empty,
            LeaveType = leave.Type.ToString(),
            Status = leave.Status.ToString(),
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            Reason = leave.Reason,
            RejectionReason = leave.RejectionReason,
            CreatedAt = leave.CreatedAt
        };

        return Result<LeaveResponse, GetLeaveByIdPossibleErrors>.Success(leaveDto);
    }

}