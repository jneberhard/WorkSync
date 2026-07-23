using System.ComponentModel.DataAnnotations;

namespace WorkSync.Data;

public class TenantRequest
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string CompanyName { get; set; } = "";

    [Required, EmailAddress, StringLength(256)]
    public string AdminEmail { get; set; } = "";

    [Required]
    public string AdminUserId { get; set; } = "";

    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAtUtc { get; set; }
    public int? AssignedTenantId { get; set; }
    public bool IsApproved { get; set; }
}
