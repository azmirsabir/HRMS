using Application.Contracts;
using Core.Domain.Employee;
using MediatR;

namespace Application.Features.Employee.Command.UpdateEmployee;

public enum UpdateEmployeePossibleErrors
{
    EmployeeNotFound,
    DepartmentNotExist
}
public class UpdateEmployeeCommand : IRequest<Result<int, UpdateEmployeePossibleErrors>>
{
    public int Id { get; set; }
    public decimal BaseSalary { get; set; }
    public int ServiceInYears { get; set; }
    public DegreeLevel Degree { get; set; }
}