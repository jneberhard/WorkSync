using Microsoft.AspNetCore.Identity;

namespace WorkSync.Data;

public static class IdentitySeedData
{
    public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = ["Admin", "Leader", "Viewer"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await CreateUserIfMissing(
            userManager,
            "admin@worksync.com",
            "Admin123!",
            "Admin");

        await CreateUserIfMissing(
            userManager,
            "leader@worksync.com",
            "Leader123!",
            "Leader");

        await CreateUserIfMissing(
            userManager,
            "viewer@worksync.com",
            "Viewer123!",
            "Viewer");
    }

    private static async Task CreateUserIfMissing(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                IsApproved = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    $"Failed to create user {email}: " +
                    string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }
        }
        else if (!user.IsApproved)
        {
            user.IsApproved = true;
            await userManager.UpdateAsync(user);
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}