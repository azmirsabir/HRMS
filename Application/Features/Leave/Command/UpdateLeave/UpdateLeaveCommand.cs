using Application.Contracts;
using Core.Domain.Employee;
using Core.Domain.Leave;
using MediatR;

namespace Application.Features.Leave.Command.UpdateLeave;

public enum UpdateLeavePossibleErrors
{
    LeaveNotFound,
    LeaveAlreadyApproved,
    InvalidEmployeeDepartment
}
public class UpdateLeaveCommand : IRequest<Result<int, UpdateLeavePossibleErrors>>
{
    public int Id { get; set; }
    public LeaveStatus Status { get; set; }
    public string? RejectionReason { get; set; }
}