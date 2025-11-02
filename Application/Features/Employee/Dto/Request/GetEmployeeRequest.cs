using Core.Domain.Employee;

namespace Application.Features.Employee.Dto.Request;

public class GetEmployeeRequest
{
    public int? DepartmentId { get; set; }
    public string? FullName { get; set; }
    
    public Gender? Gender { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}