using Core.Domain.Department;
using Core.Domain.Employee;
using Core.Domain.Leave;
using Core.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<Employee> Employees => Set<Employee>();
    DbSet<Department> Departments => Set<Department>();
    DbSet<Leave> Leaves => Set<Leave>();
    
    DbSet<User> Users => Set<User>();
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade Database { get; }
    
    DbSet<T> Set<T>() where T : class;
}