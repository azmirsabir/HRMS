using Application.Abstractions.Data;
using Application.Contracts;
using Application.Features.Leave.Dto.Response;
using Application.Interfaces;
using Core.Domain.Leave;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Leave.Query.GetAllLeave;

public enum GetAllLeavePossibleErrors
{
}

public record
    GetAllLeaveQuery : IRequest<
    Result<PaginatedResponse<LeaveResponse>, GetAllLeavePossibleErrors>>
{
    public int? EmployeeId { get; set; }
    public LeaveStatus? Status { get; set; }
    public LeaveType? Type { get; set; }
    public string? FullName { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}