using Application.Abstractions.Data;
using Application.Contracts;
using Application.Interfaces;
using Core.Domain.Leave;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Leave.Command.AddLeave;

public class AddLeaveCommandHandler : IRequestHandler<AddLeaveCommand, Result<int, AddLeavePossibleErrors>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    
    public AddLeaveCommandHandler(IApplicationDbContext applicationDbContext,ICurrentUser currentUser)
    {
        _context = applicationDbContext;
        _currentUser = currentUser;
    }
    
    public async Task<Result<int, AddLeavePossibleErrors>> Handle(AddLeaveCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == _currentUser.Id, cancellationToken);
        
        var leave = Core.Domain.Leave.Leave.Create(
            employeeId: user.EmployeeId,
            type: request.LeaveType,
            reason: request.Reason,
            status: LeaveStatus.Pending,
            startDate: request.StartDate,
            endDate: request.EndDate
        );
        
        var createdLeave=_context.Leaves.Add(leave);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result<int, AddLeavePossibleErrors>.Success(createdLeave.Entity.Id);
    }
}