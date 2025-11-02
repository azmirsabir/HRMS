using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Department;
using BaseEntity;

public class Department : BaseEntity<int>
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Employee.Employee> Employees { get; set; } = new List<Employee.Employee>();

    /// <summary>
    /// Factory method to create a new Department entity
    /// </summary>
    /// <param name="name">Name of the department</param>
    /// <returns>A new Department instance</returns>
    /// <exception cref="ArgumentException">Thrown when the name is invalid</exception>
    public static Department Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name cannot be null or empty.", nameof(name));

        return new Department
        {
            Name = name.Trim(),
            // CreatedAt = DateTime.UtcNow,
            // UpdatedAt = DateTime.UtcNow,
            // Deleted = false
        };
    }
}