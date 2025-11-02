using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;
using Core.Domain.BaseEntity;

namespace Core.Domain.Employee;

public class Employee : BaseEntity<int>
{
    [Required, MaxLength(30)]
    public string FullName { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    
    [ForeignKey(nameof(DepartmentId))]
    public Department.Department? Department { get; set; } = null!;
    public DegreeLevel CurrentDegree { get; set; }
    
    public int ServiceInYears { get; set; }
    public Gender Gender { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal BaseSalary { get; set; }
    
    
    /// <summary>
    /// Factory method to create a new Employee entity
    /// </summary>
    /// <param name="fullName">The Full Name</param>
    /// <param name="departmentId">The Department Id</param>
    /// <param name="currentDegree">The Current Degree</param>
    /// <param name="serviceInYears">The Service In Years</param>
    /// <param name="gender">The Employee Gender</param>
    /// <param name="baseSalary">The Base Salary</param>
    /// <returns>A new Employee instance</returns>
    /// <exception cref="ArgumentException">Thrown when invalid parameters are provided</exception>
    /// 
    public static Employee Create(
        string fullName,
        int departmentId,
        DegreeLevel currentDegree,
        int serviceInYears,
        Gender gender,
        decimal baseSalary = 0)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty.", nameof(fullName));

        if (departmentId <= 0)
            throw new ArgumentException("Department Id must be greater than zero.", nameof(departmentId));

        if (serviceInYears < 0)
            throw new ArgumentException("Service in years cannot be negative.", nameof(serviceInYears));

        if (baseSalary < 0)
            throw new ArgumentException("Base salary cannot be negative.", nameof(baseSalary));

        return new Employee
        {
            FullName = fullName.Trim(),
            DepartmentId = departmentId,
            CurrentDegree = currentDegree,
            ServiceInYears = serviceInYears,
            Gender = gender,
            BaseSalary = baseSalary,
            // CreatedAt = DateTime.UtcNow,
            // UpdatedAt = DateTime.UtcNow,
            // Deleted = false
        };
    }
}

public enum DegreeLevel
{
    NoDegree = 0,
    HighSchool = 1,
    Bachelor = 2,
    Master = 3,
    PhD = 4
}

public enum Gender
{
    Male = 0,
    Female = 1
}