using Application.Abstractions.Data;
using Application.Contracts;
using Application.Features.Employee.Dto;
using Application.Features.Employee.Dto.Response;
using Application.Interfaces;
using Core.Domain.Employee;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Employee.Query.GetAllEmployee;

public class GetAllEmployeeHandler : IRequestHandler<GetAllEmployeeQuery,
    Result<PaginatedResponse<EmployeeResponse>, GetAllEmployeePossibleErrors>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly ICurrentUser _currentUser;
    private readonly ISalaryCalculator _salaryCalculator;

    public GetAllEmployeeHandler(IApplicationDbContext applicationDbContext, ICurrentUser currentUser, ISalaryCalculator salaryCalculator)
    {
        _applicationDbContext = applicationDbContext;
        _currentUser = currentUser;
        _salaryCalculator = salaryCalculator;
    }

    public async Task<Result<PaginatedResponse<EmployeeResponse>, GetAllEmployeePossibleErrors>> Handle(
        GetAllEmployeeQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.Id;
        var roles = _currentUser.Roles;
        
        var query = _applicationDbContext.Employees
            .Include(e => e.Department)
            .AsQueryable();
        
        
        if (!roles.Contains("Auditor"))
        {
            var user = await _applicationDbContext.Users
                .AsNoTracking()
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

            query = roles.Contains("Employee")
                ? query.Where(e => e.Id == user.EmployeeId)
                : query.Where(e => e.DepartmentId == user.Employee.DepartmentId);

        }

        if (request.DepartmentId is > 0)
        {
            query = query.Where(e => e.DepartmentId == request.DepartmentId);
        }

        if (request.Gender.HasValue)
        {
            query = query.Where(e => e.Gender == request.Gender);
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            query = query.Where(e => e.FullName.ToLower().Contains(request.FullName.ToLower()));
        }

        var paginatedResponse = await PaginatedResponse<EmployeeResponse>.CreateAsync(
            EmployeeResponse.ToResponse(query, _salaryCalculator),
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        return Result<PaginatedResponse<EmployeeResponse>, GetAllEmployeePossibleErrors>.Success(
            paginatedResponse
        );
    }
}