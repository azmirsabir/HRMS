using Application.Abstractions.Data;
using Application.Contracts;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Department.Command.AddDepartment;

public class AddDepartmentCommandHandler : IRequestHandler<AddDepartmentCommand, Result<int, AddDepartmentPossibleErrors>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    
    public AddDepartmentCommandHandler(IApplicationDbContext applicationDbContext,ICurrentUser currentUser)
    {
        _context = applicationDbContext;
        _currentUser = currentUser;
    }
    
    public async Task<Result<int, AddDepartmentPossibleErrors>> Handle(AddDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = Core.Domain.Department.Department.Create(
            name: request.Name
        );
        
        var createdDepartment=_context.Departments.Add(department);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result<int, AddDepartmentPossibleErrors>.Success(createdDepartment.Entity.Id);
    }
}