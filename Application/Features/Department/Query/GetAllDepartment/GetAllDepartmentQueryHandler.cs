using Application.Abstractions.Data;
using Application.Contracts;
using Application.Features.Department.Dto.Response;
using MediatR;

namespace Application.Features.Department.Query.GetAllDepartment;

public class GetAllDepartmentHandler : IRequestHandler<GetAllDepartmentQuery,
    Result<PaginatedResponse<DepartmentResponse>, GetAllDepartmentPossibleErrors>>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public GetAllDepartmentHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Result<PaginatedResponse<DepartmentResponse>, GetAllDepartmentPossibleErrors>> Handle(
        GetAllDepartmentQuery request, CancellationToken cancellationToken)
    {
        var query = _applicationDbContext.Departments
            .AsQueryable();

        if (request.DepartmentId is > 0)
        {
            query = query.Where(d => d.Id == request.DepartmentId);
        }

        if (request.Name != null)
        {
            query = query.Where(e => e.Name == request.Name);
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var lowerName = request.Name.ToLower();
            query = query.Where(d => d.Name.ToLower().Contains(lowerName));
        }

        var paginatedResponse = await PaginatedResponse<DepartmentResponse>.CreateAsync(
            DepartmentResponse.ToResponse(query),
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        return Result<PaginatedResponse<DepartmentResponse>, GetAllDepartmentPossibleErrors>.Success(
            paginatedResponse
        );
    }
}