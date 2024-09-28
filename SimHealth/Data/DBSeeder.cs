using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Data;

//TODO
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
                var roles = new List<IdentityRole>() {
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
        }
    }
}
