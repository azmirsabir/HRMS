using Application.Abstractions.Data;
using Application.Contracts;
using Application.Features.Employee.Dto;
using Application.Features.Employee.Dto.Response;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Employee.Query.GetEmployeeById;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeResponse, GetEmployeeByIdPossibleErrors>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISalaryCalculator _salaryCalculator;

    public GetEmployeeByIdQueryHandler(IApplicationDbContext context, ISalaryCalculator salaryCalculator)
    {
        _context = context;
        _salaryCalculator = salaryCalculator;
    }

    public async Task<Result<EmployeeResponse, GetEmployeeByIdPossibleErrors>> Handle(
        GetEmployeeByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var employee = await _context.Set<Core.Domain.Employee.Employee>()
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee == null)
        {
            return Result<EmployeeResponse, GetEmployeeByIdPossibleErrors>
                .Failure(GetEmployeeByIdPossibleErrors.EmployeeNotFound);
        }

        var employeeDto = new EmployeeResponse
        {
            Id = employee.Id,
            FullName = employee.FullName,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name ?? string.Empty,
            CurrentDegree = employee.CurrentDegree.ToString(),
            ServiceInYears = employee.ServiceInYears,
            Gender = employee.Gender.ToString(),
            BaseSalary = employee.BaseSalary,
            TotalSalary = _salaryCalculator.CalculateTotalSalary(employee.BaseSalary,employee.CurrentDegree,employee.ServiceInYears),
            CreatedAt = employee.CreatedAt
        };

        return Result<EmployeeResponse, GetEmployeeByIdPossibleErrors>.Success(employeeDto);
    }

}