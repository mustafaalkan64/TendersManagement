using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;

public static class DbSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Seed Roles
        await SeedRolesAsync(roleManager);

        // Seed Admin User
        await SeedAdminUserAsync(userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        // Seed Roles
        if (!await roleManager.RoleExistsAsync(Roles.Admin))
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin));

        if (!await roleManager.RoleExistsAsync(Roles.User))
            await roleManager.CreateAsync(new IdentityRole(Roles.User));
    }

    private static async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager)
    {
        // Seed Admin User
        var adminUser = await userManager.FindByEmailAsync("admin@garantitarimsaldanismanlik.com");

        if (adminUser == null)
        {
            var user = new IdentityUser
            {
                UserName = "admin@garantitarimsaldanismanlik.com",
                Email = "admin@garantitarimsaldanismanlik.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Admin#!123.");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, Roles.Admin);
            }
        }
    }
} 