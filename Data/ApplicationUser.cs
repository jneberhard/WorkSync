using Microsoft.AspNetCore.Identity;

namespace WorkSync.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = false;
    public int? TenantId { get; set; }
    public Tenant? Tenant { get; set; }
}

