
namespace Application.Features.Department.Dto.Response;

public class DepartmentResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Converts IQueryable<Department> to IQueryable<DepartmentResponse>
    /// </summary>
    public static IQueryable<DepartmentResponse> ToResponse(IQueryable<Core.Domain.Department.Department> department)
    {
        return department.Select(e => new DepartmentResponse()
        {
            Id = e.Id,
            Name = e.Name,
            CreatedAt = e.CreatedAt
        });
    }
}