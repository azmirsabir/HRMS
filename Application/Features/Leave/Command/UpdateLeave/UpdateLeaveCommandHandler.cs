using Application.Abstractions.Data;
using Application.Contracts;
using Application.Interfaces;
using Core.Domain.Leave;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Leave.Command.UpdateLeave;

public class UpdateLeaveCommandHandler : IRequestHandler<UpdateLeaveCommand, Result<int, UpdateLeavePossibleErrors>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IApplicationDbContext _context;
    
    public UpdateLeaveCommandHandler(ICurrentUser currentUser, IApplicationDbContext applicationDbContext)
    {
        _currentUser = currentUser;
        _context = applicationDbContext;
    }
    
    public async Task<Result<int, UpdateLeavePossibleErrors>> Handle(UpdateLeaveCommand request, CancellationToken cancellationToken)
    {
        var leave = await _context.Leaves
            .Include(l => l.Employee)
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (leave == null)
        {
            return Result<int, UpdateLeavePossibleErrors>.Failure(UpdateLeavePossibleErrors.LeaveNotFound);
        }
        
        if (leave.Status != LeaveStatus.Pending)
        {
            return Result<int, UpdateLeavePossibleErrors>.Failure(UpdateLeavePossibleErrors.LeaveAlreadyApproved);
        }

        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Id == _currentUser.Id, cancellationToken);

        if (user.Employee.DepartmentId != leave.Employee.DepartmentId)
        {
            return Result<int, UpdateLeavePossibleErrors>.Failure(UpdateLeavePossibleErrors.InvalidEmployeeDepartment);
        }
        
        leave.Status = request.Status;
        leave.RejectionReason = request.RejectionReason;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int, UpdateLeavePossibleErrors>.Success(leave.Id);
    }
}