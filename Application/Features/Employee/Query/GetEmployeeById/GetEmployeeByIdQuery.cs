using Application.Abstractions.Data;
using Application.Contracts;
using Application.Features.Employee.Dto;
using Application.Features.Employee.Dto.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Employee.Query.GetEmployeeById;

public enum GetEmployeeByIdPossibleErrors
{
    EmployeeNotFound
}

public class GetEmployeeByIdQuery : IRequest<Result<EmployeeResponse, GetEmployeeByIdPossibleErrors>>
{
    public int EmployeeId { get; set; }

    public GetEmployeeByIdQuery(int employeeId)
    {
        EmployeeId = employeeId;
    }
}