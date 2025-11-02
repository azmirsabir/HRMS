using Core.Domain.Department;
using Core.Domain.Employee;
using Core.Domain.Users;
using Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace Test.Database.Seeds;

public static class DataSeed
{
    public static async Task<(MainContext context, User aliUser, Employee aliEmployee, User managerUser, Employee managerEmployee, Employee auditorEmployee, User auditorUser)> SeedTestDataAsync()
    {
        var options = new DbContextOptionsBuilder<MainContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new MainContext(options);

        // Department
        var department = Department.Create("IT");
        context.Departments.Add(department);

        // Ali
        var aliEmployee = Employee.Create("Ali", department.Id, DegreeLevel.Master, 9, Gender.Male, 500);
        context.Employees.Add(aliEmployee);

        var aliUser = User.Create("Ali", "ali123", aliEmployee.Id);
        aliUser.Id = "aliUser";
        context.Users.Add(aliUser);
        
        //Auditor
        var auditorEmployee = Employee.Create("Auditor", department.Id, DegreeLevel.Master, 15, Gender.Male, 1500);
        context.Employees.Add(aliEmployee);
        
        var auditorUser = User.Create("Auditor", "auditor123", auditorEmployee.Id);
        auditorUser.Id = "auditorUser";
        context.Users.Add(auditorUser);

        // Manager
        var managerEmployee = Employee.Create("Manager", department.Id, DegreeLevel.Master, 9, Gender.Male, 500);
        context.Employees.Add(managerEmployee);

        var managerUser = User.Create("Manager", "manager123", managerEmployee.Id);
        managerUser.Id = "managerUser";
        context.Users.Add(managerUser);

        // Save
        await context.SaveChangesAsync();

        return (context, aliUser, aliEmployee, managerUser, managerEmployee, auditorEmployee, auditorUser);
    }
}