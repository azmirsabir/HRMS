using Core.Domain.Leave;

namespace Application.Features.Leave.Dto.Request;

public class GetLeaveRequest
{
    public int? EmployeeId { get; set; }
    public string? FullName { get; set; }
    
    public LeaveStatus? Status { get; set; }
    public LeaveType? Type { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}