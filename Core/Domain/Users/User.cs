using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Core.Domain.BaseEntity;

namespace Core.Domain.Users;

public class User : IdentityUser
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    public int EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public Employee.Employee? Employee { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool Deleted { get; set; } = false;

    public User(string fullName, string userName, int employeeId, string? email = null, string? phoneNumber = null,
        bool emailConfirmed = false)
    {
        FullName = fullName;
        UserName = userName;
        EmployeeId = employeeId;
        Email = email;
        PhoneNumber = phoneNumber;
        EmailConfirmed = emailConfirmed;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Deleted = false;
    }


    public User()
    {
    }

    /// <summary>
    /// Factory method to create a new ApplicationUser entity
    /// </summary>
    /// <param name="fullName">The full name of the user</param>
    /// <param name="userName">The username</param>
    /// <param name="employeeId">The employeeId</param>
    /// <param name="email">Optional email address</param>
    /// <param name="phoneNumber">Optional phone number</param>
    /// <param name="emailConfirmed">Whether email is confirmed (defaults to false)</param>
    /// <returns>A new ApplicationUser instance</returns>
    /// <exception cref="ArgumentException">Thrown when required parameters are invalid</exception>
    public static User Create(
        string fullName,
        string userName,
        int employeeId,
        string? email = null,
        string? phoneNumber = null,
        bool emailConfirmed = false)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Full name cannot be null or empty", nameof(fullName));
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException("Username cannot be null or empty", nameof(userName));
        }
        
        if (int.IsNegative(employeeId))
        {
            throw new ArgumentException("employeeId cannot be negative", nameof(employeeId));
        }
        
        return new User
        {
            FullName = fullName.Trim(),
            UserName = userName.Trim(),
            EmployeeId = employeeId,
            Email = email?.Trim().ToLowerInvariant(),
            PhoneNumber = phoneNumber,
            EmailConfirmed = emailConfirmed,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Deleted = false
        };
    }

    public void UpdateUser(string fullName, string userName, string phoneNumber, string? email = null)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
            FullName = fullName.Trim();

        if (!string.IsNullOrWhiteSpace(userName))
            UserName = userName.Trim();

        PhoneNumber = phoneNumber;
        Email = email?.Trim().ToLowerInvariant();

        UpdatedAt = DateTime.UtcNow;
    }
}



public enum UserRole
{
    Employee = 0,
    Manager = 1,
    Auditor = 2,
}