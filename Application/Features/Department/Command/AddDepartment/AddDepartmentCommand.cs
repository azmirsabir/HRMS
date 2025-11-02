using Application.Contracts;
using MediatR;

namespace Application.Features.Department.Command.AddDepartment;

public enum AddDepartmentPossibleErrors
{
    
}
public class AddDepartmentCommand : IRequest<Result<int, AddDepartmentPossibleErrors>>
{
    public string? Name { get; set; }
}