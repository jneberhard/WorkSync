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
    [FromForm] string adminEmail,
    ApplicationDbContext db) =>
{
    companyName = companyName.Trim();
    adminEmail = adminEmail.Trim().ToLowerInvariant();

    if (string.IsNullOrWhiteSpace(companyName) ||
        !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(adminEmail))
    {
        return Results.LocalRedirect(
            "/Account/RequestTenant?message=Enter+a+valid+company+name+and+administrator+email.");
    }

    var alreadyExists =
        await db.TenantRequests.AnyAsync(request =>
            !request.IsApproved && request.AdminEmail == adminEmail) ||
        await db.Tenants.AnyAsync(tenant => tenant.AdminEmail == adminEmail);

    if (!alreadyExists)
    {
        db.TenantRequests.Add(new TenantRequest
        {
            CompanyName = companyName,
            AdminEmail = adminEmail
        });
        await db.SaveChangesAsync();
    }

    return Results.LocalRedirect(
        "/Account/RequestTenant?message=Your+request+was+submitted.+The+super+user+will+provide+the+tenant+ID+after+approval.");
});

app.MapPost("/SuperAdmin/ApproveTenant", async (
    [FromForm] int requestId,
    ApplicationDbContext db) =>
{
    var request = await db.TenantRequests.SingleOrDefaultAsync(item => item.Id == requestId);
    if (request is null || request.IsApproved)
    {
        return TenantManagementRedirect("The tenant request was not found.", false);
    }

    int tenantId;
    do
    {
        tenantId = RandomNumberGenerator.GetInt32(100000, 1000000);
    }
    while (await db.Tenants.AnyAsync(tenant => tenant.Id == tenantId));

    db.Tenants.Add(new Tenant
    {
        Id = tenantId,
        Name = request.CompanyName,
        AdminEmail = request.AdminEmail,
        IsApproved = true,
        IsActive = false,
        ApprovedAtUtc = DateTime.UtcNow
    });
    request.IsApproved = true;
    request.ApprovedAtUtc = DateTime.UtcNow;
    request.AssignedTenantId = tenantId;
    await db.SaveChangesAsync();

    return TenantManagementRedirect(
        $"Approved {request.CompanyName}. Tenant ID: {tenantId}. Give this ID to {request.AdminEmail}.",
        true);
}).RequireAuthorization(policy => policy.RequireRole("SuperUser"));

app.MapPost("/SuperAdmin/ApproveTenantAdmin", async (
    [FromForm] string userId,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext db) =>
{
    var user = await userManager.FindByIdAsync(userId);
    var tenant = user?.TenantId is int tenantId
        ? await db.Tenants.SingleOrDefaultAsync(item => item.Id == tenantId)
        : null;

    if (user is null || tenant is null ||
        !string.Equals(user.Email, tenant.AdminEmail, StringComparison.OrdinalIgnoreCase))
    {
        return TenantManagementRedirect("The tenant administrator registration was not found.", false);
    }

    if (!await userManager.IsInRoleAsync(user, "Admin"))
    {
        var roleResult = await userManager.AddToRoleAsync(user, "Admin");
        if (!roleResult.Succeeded)
        {
            return TenantManagementRedirect(
                string.Join(" ", roleResult.Errors.Select(error => error.Description)), false);
        }
    }

    user.IsApproved = true;
    tenant.IsActive = true;
    var updateResult = await userManager.UpdateAsync(user);
    if (!updateResult.Succeeded)
    {
        return TenantManagementRedirect(
            string.Join(" ", updateResult.Errors.Select(error => error.Description)), false);
    }

    await db.SaveChangesAsync();
    return TenantManagementRedirect($"Activated {tenant.Name} and approved {user.Email} as its administrator.", true);
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
