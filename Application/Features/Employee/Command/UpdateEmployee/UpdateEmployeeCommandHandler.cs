using Application.Abstractions.Data;
using Application.Contracts;
using Application.Features.Employee.Command.UpdateEmployee;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Employee.Command.UpdateEmployee;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result<int, UpdateEmployeePossibleErrors>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IApplicationDbContext _context;
    
    public UpdateEmployeeCommandHandler(ICurrentUser currentUser, IApplicationDbContext applicationDbContext)
    {
        _currentUser = currentUser;
        _context = applicationDbContext;
    }
    
    public async Task<Result<int, UpdateEmployeePossibleErrors>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (employee == null)
        {
            return Result<int, UpdateEmployeePossibleErrors>.Failure(UpdateEmployeePossibleErrors.EmployeeNotFound);
        }
        
        employee.CurrentDegree = request.Degree;
        employee.ServiceInYears = request.ServiceInYears;
        employee.BaseSalary = request.BaseSalary;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int, UpdateEmployeePossibleErrors>.Success(employee.Id);
    }
}