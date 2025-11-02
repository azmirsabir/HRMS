using Core.Domain.Employee;
using Core.Domain.Leave;

namespace Application.Features.Leave.Dto.Response;

public class LeaveResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string LeaveType { get; set; }
    public string Status { get; set; }
    public string? Reason { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// Converts IQueryable<Leave> to IQueryable<LeaveResponse>
    /// </summary>
    public static IQueryable<LeaveResponse> ToResponse(IQueryable<Core.Domain.Leave.Leave> leaves)
    {
        return leaves.Select(e => new LeaveResponse()
        {
            Id = e.Id,
            EmployeeId = e.EmployeeId,
            EmployeeName = e.Employee != null ? e.Employee.FullName : string.Empty,
            DepartmentName  = e.Employee.Department != null ? e.Employee.Department.Name : string.Empty,
            LeaveType = e.Type.ToString(),
            Status = e.Status.ToString(),
            Reason = e.Reason!=null ? e.Reason : "",
            RejectionReason = e.RejectionReason!=null ? e.RejectionReason : "",
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            CreatedAt = e.CreatedAt
        });
    }
}