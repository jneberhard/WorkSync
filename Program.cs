using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkSync.Components;
using WorkSync.Components.Account;
using WorkSync.Data;
using WorkSync.Services;

var builder = WebApplication.CreateBuilder(args);

if (Environment.GetEnvironmentVariable("PORT") is { Length: > 0 } port)
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "A PostgreSQL connection string is required. Set ConnectionStrings__DefaultConnection.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure()));

// Persist cookie-encryption keys in PostgreSQL so every stateless production
// instance can read authentication cookies issued by another instance.
builder.Services.AddDataProtection()
    .SetApplicationName("WorkSync")
    .PersistKeysToDbContext<ApplicationDbContext>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddClaimsPrincipalFactory<TenantClaimsPrincipalFactory>()
    .AddDefaultTokenProviders();

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
    options.ValidationInterval = TimeSpan.FromMinutes(1));

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddScoped<WorkSyncService>();
builder.Services.AddScoped<CalendarExportService>();

var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Vercel's static asset layer does not expose the underscore-prefixed
// /_framework path, so serve the Blazor runtime through an application route.
app.MapGet("/blazor.web.js", (IWebHostEnvironment environment) =>
    Results.File(
        Path.Combine(environment.WebRootPath, "_framework", "blazor.web.js"),
        "text/javascript; charset=utf-8"));

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapPost("/Account/RequestTenant", async (
    [FromForm] string companyName,
    [FromForm] string firstName,
    [FromForm] string lastName,
    [FromForm] string adminEmail,
    [FromForm] string password,
    [FromForm] string confirmPassword,
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager) =>
{
    companyName = companyName.Trim();
    firstName = firstName.Trim();
    lastName = lastName.Trim();
    adminEmail = adminEmail.Trim().ToLowerInvariant();

    if (string.IsNullOrWhiteSpace(companyName) ||
        string.IsNullOrWhiteSpace(firstName) ||
        string.IsNullOrWhiteSpace(lastName) ||
        !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(adminEmail))
    {
        return RequestTenantRedirect(
            "Enter a company name, administrator name, and valid email.");
    }

    if (password != confirmPassword)
    {
        return RequestTenantRedirect("The password and confirmation password do not match.");
    }

    var existingRequest = await db.TenantRequests.SingleOrDefaultAsync(request =>
        !request.IsApproved && request.AdminEmail == adminEmail);
    var alreadyExists =
        await userManager.FindByEmailAsync(adminEmail) is not null ||
        await db.Tenants.AnyAsync(tenant => tenant.AdminEmail == adminEmail) ||
        !string.IsNullOrEmpty(existingRequest?.AdminUserId);

    if (alreadyExists)
    {
        return RequestTenantRedirect(
            "An account or tenant request already exists for that administrator email.");
    }

    var adminUser = new ApplicationUser
    {
        UserName = adminEmail,
        Email = adminEmail,
        FirstName = firstName,
        LastName = lastName,
        IsApproved = false,
        EmailConfirmed = false,
        TenantId = null
    };

    var createResult = await userManager.CreateAsync(adminUser, password);
    if (!createResult.Succeeded)
    {
        return RequestTenantRedirect(
            string.Join(" ", createResult.Errors.Select(error => error.Description)));
    }

    try
    {
        if (existingRequest is null)
        {
            db.TenantRequests.Add(new TenantRequest
            {
                CompanyName = companyName,
                AdminEmail = adminEmail,
                AdminUserId = adminUser.Id
            });
        }
        else
        {
            // Upgrade a request created before administrator credentials were required.
            existingRequest.CompanyName = companyName;
            existingRequest.AdminUserId = adminUser.Id;
        }
        await db.SaveChangesAsync();
    }
    catch
    {
        await userManager.DeleteAsync(adminUser);
        throw;
    }

    return RequestTenantRedirect(
        "Your administrator account and tenant request were submitted. You can sign in after the super user approves them.");
});

app.MapPost("/SuperAdmin/ApproveTenant", async (
    [FromForm] int requestId,
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager) =>
{
    var request = await db.TenantRequests.SingleOrDefaultAsync(item => item.Id == requestId);
    if (request is null || request.IsApproved)
    {
        return TenantManagementRedirect("The tenant request was not found.", false);
    }

    var adminUser = await userManager.FindByIdAsync(request.AdminUserId);
    if (adminUser is null ||
        !string.Equals(adminUser.Email, request.AdminEmail, StringComparison.OrdinalIgnoreCase))
    {
        return TenantManagementRedirect(
            "The pending administrator account is missing. Submit a new tenant request.", false);
    }

    int tenantId;
    do
    {
        tenantId = RandomNumberGenerator.GetInt32(100000, 1000000);
    }
    while (await db.Tenants.AnyAsync(tenant => tenant.Id == tenantId));

    await using var transaction = await db.Database.BeginTransactionAsync();

    var tenant = new Tenant
    {
        Id = tenantId,
        Name = request.CompanyName,
        AdminEmail = request.AdminEmail,
        IsApproved = true,
        IsActive = true,
        ApprovedAtUtc = DateTime.UtcNow
    };
    db.Tenants.Add(tenant);
    await db.SaveChangesAsync();

    adminUser.TenantId = tenantId;
    adminUser.IsApproved = true;
    adminUser.EmailConfirmed = true;
    var updateResult = await userManager.UpdateAsync(adminUser);
    if (!updateResult.Succeeded)
    {
        await transaction.RollbackAsync();
        return TenantManagementRedirect(
            string.Join(" ", updateResult.Errors.Select(error => error.Description)), false);
    }

    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
        if (!roleResult.Succeeded)
        {
            await transaction.RollbackAsync();
            return TenantManagementRedirect(
                string.Join(" ", roleResult.Errors.Select(error => error.Description)), false);
        }
    }

    request.IsApproved = true;
    request.ApprovedAtUtc = DateTime.UtcNow;
    request.AssignedTenantId = tenantId;
    await db.SaveChangesAsync();
    await transaction.CommitAsync();

    return TenantManagementRedirect(
        $"Activated {request.CompanyName} with tenant ID {tenantId} and approved {request.AdminEmail} as Admin.",
        true);
}).RequireAuthorization(policy => policy.RequireRole("SuperUser"));

app.MapPost("/SuperAdmin/SetTenantArchived", async (
    [FromForm] int tenantId,
    [FromForm] bool archive,
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager) =>
{
    var tenant = await db.Tenants.SingleOrDefaultAsync(item => item.Id == tenantId);
    if (tenant is null)
    {
        return TenantManagementRedirect("The tenant was not found.", false);
    }

    if (tenant.IsActive == !archive)
    {
        return TenantManagementRedirect(
            $"{tenant.Name} is already {(archive ? "archived" : "active")}.",
            true);
    }

    if (archive)
    {
        var tenantUsers = await db.Users
            .Where(user => user.TenantId == tenantId)
            .ToListAsync();
        foreach (var user in tenantUsers)
        {
            var stampResult = await userManager.UpdateSecurityStampAsync(user);
            if (!stampResult.Succeeded)
            {
                return TenantManagementRedirect(
                    $"Could not archive {tenant.Name}. " +
                    string.Join(" ", stampResult.Errors.Select(error => error.Description)),
                    false);
            }
        }
    }

    tenant.IsActive = !archive;
    tenant.ArchivedAtUtc = archive ? DateTime.UtcNow : null;
    await db.SaveChangesAsync();

    return TenantManagementRedirect(
        archive
            ? $"Archived {tenant.Name} ({tenant.Id}). All users are blocked, and all workspace data was preserved."
            : $"Restored {tenant.Name} ({tenant.Id}). Its users can sign in again with all workspace data intact.",
        true);
}).RequireAuthorization(policy => policy.RequireRole("SuperUser"));

app.MapPost("/SuperAdmin/RemoveTenant", async (
    [FromForm] int tenantId,
    [FromForm] string tenantIdConfirmation,
    ApplicationDbContext db) =>
{
    if (!int.TryParse(tenantIdConfirmation, out var confirmedTenantId) ||
        confirmedTenantId != tenantId)
    {
        return TenantManagementRedirect(
            "Tenant removal was cancelled because the confirmation ID did not match.", false);
    }

    var tenant = await db.Tenants.AsNoTracking()
        .SingleOrDefaultAsync(item => item.Id == tenantId);
    if (tenant is null)
    {
        return TenantManagementRedirect("The tenant was not found.", false);
    }

    await using var transaction = await db.Database.BeginTransactionAsync();
    await db.Workorders.Where(item => item.TenantId == tenantId).ExecuteDeleteAsync();
    await db.Assignments.Where(item => item.TenantId == tenantId).ExecuteDeleteAsync();
    await db.FollowUpItems.Where(item => item.TenantId == tenantId).ExecuteDeleteAsync();
    await db.Members.Where(item => item.TenantId == tenantId).ExecuteDeleteAsync();
    await db.Users.Where(user => user.TenantId == tenantId).ExecuteDeleteAsync();
    await db.TenantRequests
        .Where(request => request.AssignedTenantId == tenantId)
        .ExecuteDeleteAsync();
    await db.Tenants.Where(item => item.Id == tenantId).ExecuteDeleteAsync();
    await transaction.CommitAsync();

    return TenantManagementRedirect(
        $"Removed tenant {tenant.Name} ({tenant.Id}) and all of its users and workspace data.",
        true);
}).RequireAuthorization(policy => policy.RequireRole("SuperUser"));

app.MapPost("/Admin/ApproveUser", async (
    [FromForm] string userId,
    [FromForm] string role,
    ClaimsPrincipal currentPrincipal,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ApplicationDbContext db) =>
{
    static IResult RedirectWithMessage(string message, bool succeeded)
    {
        var query = QueryString.Create(
        [
            new KeyValuePair<string, string?>("approvalMessage", message),
            new KeyValuePair<string, string?>("approvalSucceeded", succeeded.ToString())
        ]);

        return Results.LocalRedirect($"/Admin/ManageUsers{query}");
    }

    if (role is not ("Admin" or "Leader" or "Viewer"))
    {
        return RedirectWithMessage("The selected role is invalid.", false);
    }

    var user = await userManager.FindByIdAsync(userId);
    var currentUser = await userManager.GetUserAsync(currentPrincipal);

    if (user is null ||
        currentUser?.TenantId is not int currentTenantId ||
        user.TenantId != currentTenantId)
    {
        return RedirectWithMessage("The account could not be found in your tenant.", false);
    }

    var tenantIsActive = await db.Tenants
        .AnyAsync(tenant => tenant.Id == currentTenantId && tenant.IsActive);
    if (!tenantIsActive)
    {
        return RedirectWithMessage("Your tenant is not active.", false);
    }

    if (!await roleManager.RoleExistsAsync(role))
    {
        return RedirectWithMessage($"The '{role}' role is not configured.", false);
    }

    if (!await userManager.IsInRoleAsync(user, role))
    {
        var roleResult = await userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            return RedirectWithMessage(
                string.Join(" ", roleResult.Errors.Select(error => error.Description)),
                false);
        }
    }

    user.IsApproved = true;
    var updateResult = await userManager.UpdateAsync(user);

    if (!updateResult.Succeeded)
    {
        return RedirectWithMessage(
            string.Join(" ", updateResult.Errors.Select(error => error.Description)),
            false);
    }

    return RedirectWithMessage(
        $"Successfully approved account access for {user.Email} as '{role}'.",
        true);
}).RequireAuthorization(policy => policy.RequireRole("Admin"));

static IResult TenantManagementRedirect(string message, bool succeeded)
{
    var query = QueryString.Create(
    [
        new KeyValuePair<string, string?>("message", message),
        new KeyValuePair<string, string?>("succeeded", succeeded.ToString())
    ]);
    return Results.LocalRedirect($"/SuperAdmin/Tenants{query}");
}

static IResult RequestTenantRedirect(string message)
{
    var query = QueryString.Create("message", message);
    return Results.LocalRedirect($"/Account/RequestTenant{query}");
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await IdentitySeedData.SeedRolesAndUsersAsync(scope.ServiceProvider);
}



app.MapGet("/assignments/calendar/{id:int}", async (
    int id,
    WorkSyncService workSyncService,
    CalendarExportService calendarService) =>
{
    var assignment = await workSyncService.GetAssignmentByIdAsync(id);

    if (assignment is null)
    {
        return Results.NotFound();
    }

    var calendarContent =
        calendarService.CreateAssignmentCalendarFile(assignment);

    var fileName =
        $"WorkSync-Assignment-{assignment.Id}.ics";

    return Results.File(
        System.Text.Encoding.UTF8.GetBytes(calendarContent),
        "text/calendar",
        fileName);
});

app.Run();
