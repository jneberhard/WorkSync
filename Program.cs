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

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapPost("/Admin/ApproveUser", async (
    [FromForm] string userId,
    [FromForm] string role,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) =>
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

    if (user is null)
    {
        return RedirectWithMessage("The account could not be found.", false);
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
