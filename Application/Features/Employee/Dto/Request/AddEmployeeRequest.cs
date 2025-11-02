using Core.Domain.Employee;

namespace Application.Features.Employee.Dto.Request;

public class AddEmployeeRequest
{
    public string? FullName { get; set; }
    public decimal BaseSalary { get; set; }
    public Gender Gender { get; set; }
    public int ServiceInYears { get; set; }
    public DegreeLevel Degree { get; set; }
}