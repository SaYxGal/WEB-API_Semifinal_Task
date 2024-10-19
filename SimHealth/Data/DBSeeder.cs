using AuthenticationService.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Data;
public class DBSeeder
{
    public static async Task Seed(IServiceProvider serviceProvider)
    {
        using (var context = new DataContext(
            serviceProvider.GetRequiredService<DbContextOptions<DataContext>>()))
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (!context.Roles.Any())
            {
                var roles = new List<IdentityRole>()
                {
                    new IdentityRole("Admin"),
                    new IdentityRole("Doctor"),
                    new IdentityRole("User"),
                    new IdentityRole("Manager")
                };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
            }

            var userManager = serviceProvider.GetService<UserManager<User>>();
            var passwordHasher = serviceProvider.GetService<IPasswordHasher<User>>();

            if (!context.Users.Any())
            {
                var users = new List<User>()
                {
                    new()
                    {
                        UserName = "admin",
                        NormalizedUserName = "ADMIN",
                        SecurityStamp = Guid.NewGuid().ToString()
                    },
                    new()
                    {
                        UserName = "user",
                        NormalizedUserName = "USER",
                        SecurityStamp = Guid.NewGuid().ToString()
                    },
                    new()
                    {
                        UserName = "manager",
                        NormalizedUserName = "MANAGER",
                        SecurityStamp = Guid.NewGuid().ToString()
                    },
                    new()
                    {
                        UserName = "doctor",
                        NormalizedUserName = "DOCTOR",
                        SecurityStamp = Guid.NewGuid().ToString()
                    },
                };

                foreach (var user in users)
                {
                    user.PasswordHash = passwordHasher.HashPassword(user, user.UserName);
                    context.Users.Add(user);
                }

                context.SaveChanges();

                var userItem = await userManager.FindByNameAsync("user");

                await userManager.AddToRoleAsync(userItem, UserRole.User);

                var doctorItem = await userManager.FindByNameAsync("doctor");

                await userManager.AddToRoleAsync(doctorItem, UserRole.Doctor);

                var managerItem = await userManager.FindByNameAsync("manager");

                await userManager.AddToRoleAsync(managerItem, UserRole.Manager);

                var adminItem = await userManager.FindByNameAsync("admin");

                await userManager.AddToRoleAsync(adminItem, UserRole.Admin);
            }
        }
    }
}
