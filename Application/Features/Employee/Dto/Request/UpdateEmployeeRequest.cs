using Core.Domain.Employee;

namespace Application.Features.Employee.Dto.Request;

public class UpdateEmployeeRequest
{
    public decimal BaseSalary { get; set; }
    public int ServiceInYears { get; set; }
    public DegreeLevel Degree { get; set; }
}