using Core.Domain.Leave;

namespace Application.Features.Department.Dto.Request;

public class GetDepartmentRequest
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}