using Core.Domain.Authentications.Enums;
using Core.Domain.Employee;
using Core.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Database.Seeds;

public static class UserSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (Roles role in Enum.GetValues(typeof(Roles)))
        {
            string roleName = role.ToString();
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        int employeeId = 1;
        foreach (Roles role in Enum.GetValues(typeof(Roles)))
        {
            string roleName = role.ToString();
            string email = $"azmir@{roleName.ToLower()}.com";

            var defaultUser = new User
            (
                $"Nas {roleName}",
                roleName,
                employeeId,
                email,
                ""
            )
            {
                EmailConfirmed = true
            };

            employeeId++;


            // Check if the user already exists
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                var createUserResult = await userManager.CreateAsync(defaultUser, "Azmir@123123");

                if (createUserResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultUser, roleName);
                }
            }
        }
    }
}