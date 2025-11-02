using Application.Contracts;
using Core.Domain.Employee;
using MediatR;

namespace Application.Features.Employee.Command.AddEmployee;

public enum AddEmployeePossibleErrors
{
    
}
public class AddEmployeeCommand : IRequest<Result<int, AddEmployeePossibleErrors>>
{
    public string? FullName { get; set; }
    public decimal BaseSalary { get; set; }
    public Gender Gender { get; set; }
    public int DepartmentId { get; set; }
    public int ServiceInYears { get; set; }
    public DegreeLevel Degree { get; set; }
}