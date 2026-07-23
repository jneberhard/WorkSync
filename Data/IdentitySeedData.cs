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

        string[] roles = ["SuperUser", "Admin", "Leader", "Viewer"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                EnsureSucceeded(
                    await roleManager.CreateAsync(new IdentityRole(role)),
                    $"create role {role}");
            }
        }

        await CreateUserIfMissing(
            userManager,
            "admin@worksync.com",
            "Admin123!",
            "Admin",
            tenantId: 3735930,
            enforcePassword: true);

        await CreateUserIfMissing(
            userManager,
            "leader@worksync.com",
            "Leader123!",
            "Leader",
            tenantId: 3735930);

        await CreateUserIfMissing(
            userManager,
            "viewer@worksync.com",
            "Viewer123!",
            "Viewer",
            tenantId: 3735930);

        await CreateUserIfMissing(
            userManager,
            "superuser@worksync.com",
            "SuperUser123!",
            "SuperUser",
            tenantId: null,
            enforcePassword: true);
    }

    private static async Task CreateUserIfMissing(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role,
        int? tenantId,
        bool enforcePassword = false)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                IsApproved = true,
                TenantId = tenantId
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
        else
        {
            var userNeedsUpdate = false;

            if (user.TenantId != tenantId)
            {
                user.TenantId = tenantId;
                userNeedsUpdate = true;
            }

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                userNeedsUpdate = true;
            }

            if (!user.IsApproved)
            {
                user.IsApproved = true;
                userNeedsUpdate = true;
            }

            // Keep the requested production admin account usable with its
            // documented password-only credentials.
            if (enforcePassword &&
                !string.Equals(user.UserName, email, StringComparison.OrdinalIgnoreCase))
            {
                user.UserName = email;
                userNeedsUpdate = true;
            }

            if (enforcePassword && user.TwoFactorEnabled)
            {
                user.TwoFactorEnabled = false;
                userNeedsUpdate = true;
            }

            if (enforcePassword &&
                (user.LockoutEnd is not null || user.AccessFailedCount != 0))
            {
                user.LockoutEnd = null;
                user.AccessFailedCount = 0;
                userNeedsUpdate = true;
            }

            if (userNeedsUpdate)
            {
                EnsureSucceeded(
                    await userManager.UpdateAsync(user),
                    $"update user {email}");
            }

            if (enforcePassword && !await userManager.CheckPasswordAsync(user, password))
            {
                var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                EnsureSucceeded(
                    await userManager.ResetPasswordAsync(user, resetToken, password),
                    $"reset password for {email}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            EnsureSucceeded(
                await userManager.AddToRoleAsync(user, role),
                $"add {email} to role {role}");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (result.Succeeded)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Failed to {operation}: " +
            string.Join(", ", result.Errors.Select(error => error.Description)));
    }
}
