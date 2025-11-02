using Application.Interfaces;

namespace Application.Features.Employee.Dto.Response;

public class EmployeeResponse
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string CurrentDegree { get; set; } = string.Empty;
    public int ServiceInYears { get; set; }
    public string Gender { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    
    public decimal TotalSalary { get; set; }
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Converts IQueryable<Employee> to IQueryable<GetEmployeeByIdResponse>
    /// </summary>
    public static IQueryable<EmployeeResponse> ToResponse(IQueryable<Core.Domain.Employee.Employee> employees, ISalaryCalculator salaryCalculator)
    {
        return employees.Select(e => new EmployeeResponse()
        {
            Id = e.Id,
            FullName = e.FullName,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department != null ? e.Department.Name : string.Empty,
            CurrentDegree = e.CurrentDegree.ToString(),
            ServiceInYears = e.ServiceInYears,
            Gender = e.Gender.ToString(),
            BaseSalary = e.BaseSalary,
            TotalSalary = salaryCalculator.CalculateTotalSalary(e.BaseSalary, e.CurrentDegree,e.ServiceInYears),
            CreatedAt = e.CreatedAt
        });
    }
}