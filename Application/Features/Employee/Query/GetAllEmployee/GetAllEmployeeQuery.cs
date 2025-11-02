using Application.Contracts;
using Application.Features.Employee.Dto;
using Application.Features.Employee.Dto.Response;
using Core.Domain.Employee;
using MediatR;

namespace Application.Features.Employee.Query.GetAllEmployee;

public enum GetAllEmployeePossibleErrors
{
}

public record
    GetAllEmployeeQuery : IRequest<
    Result<PaginatedResponse<EmployeeResponse>, GetAllEmployeePossibleErrors>>
{
    public int? DepartmentId { get; set; }
    public Gender? Gender { get; set; }
    public string? FullName { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}