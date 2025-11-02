using Application.Abstractions.Data;
using Application.Contracts;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Employee.Command.AddEmployee;

public class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, Result<int, AddEmployeePossibleErrors>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IApplicationDbContext _context;
    
    public AddEmployeeCommandHandler(ICurrentUser currentUser, IApplicationDbContext applicationDbContext)
    {
        _currentUser = currentUser;
        _context = applicationDbContext;
    }
    
    public async Task<Result<int, AddEmployeePossibleErrors>> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = Core.Domain.Employee.Employee.Create(
            fullName: request.FullName,
            departmentId: request.DepartmentId,
            currentDegree: request.Degree,
            serviceInYears: request.ServiceInYears,
            gender: request.Gender,
            baseSalary: request.BaseSalary
        );
        
        var createdEmp=_context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result<int, AddEmployeePossibleErrors>.Success(createdEmp.Entity.Id);
    }
}