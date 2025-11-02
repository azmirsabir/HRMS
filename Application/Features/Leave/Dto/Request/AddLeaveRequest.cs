using System.ComponentModel.DataAnnotations;
using Core.Domain.Employee;
using Core.Domain.Leave;

namespace Application.Features.Leave.Dto.Request;

public class AddLeaveRequest
{
    public LeaveType LeaveType { get; set; }
    public string? Reason { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}