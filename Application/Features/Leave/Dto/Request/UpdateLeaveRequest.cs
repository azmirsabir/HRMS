using Core.Domain.Leave;

namespace Application.Features.Leave.Dto.Request;

public class UpdateLeaveRequest
{
    public LeaveStatus status { get; set; }
    public string RejectionReason { get; set; }
}