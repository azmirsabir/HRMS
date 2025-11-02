using Core.Domain.Department;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Database.Seeds;

public class DepartmentSeed 
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DbContext>();

        if (await context.Set<Department>().AnyAsync())
            return;

        var departments = new List<Department>
        {
            new Department { Id = 1, Name = "Human Resources" },
            new Department { Id = 2, Name = "Finance" },
            new Department { Id = 3, Name = "IT" },
            new Department { Id = 4, Name = "Marketing" },
            new Department { Id = 5, Name = "Operations" },
        };

        await context.Set<Department>().AddRangeAsync(departments);
        await context.SaveChangesAsync();
    }
}