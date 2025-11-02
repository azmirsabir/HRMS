using Application.Contracts;
using Core.Domain.Leave;
using MediatR;

namespace Application.Features.Leave.Command.AddLeave;

public enum AddLeavePossibleErrors
{
    
}
public class AddLeaveCommand : IRequest<Result<int, AddLeavePossibleErrors>>
{
    public LeaveType LeaveType { get; set; }
    public string? Reason { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}