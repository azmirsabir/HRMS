using Application.Contracts;
using Application.Features.Department.Dto.Response;
using MediatR;

namespace Application.Features.Department.Query.GetAllDepartment;

public enum GetAllDepartmentPossibleErrors
{
}

public record
    GetAllDepartmentQuery : IRequest<
    Result<PaginatedResponse<DepartmentResponse>, GetAllDepartmentPossibleErrors>>
{
    public int? DepartmentId { get; set; }
    public string? Name { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}