using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkSync.Data;

public class Tenant
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = "";

    [Required, EmailAddress, StringLength(256)]
    public string AdminEmail { get; set; } = "";

    public bool IsApproved { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAtUtc { get; set; }

    public ICollection<ApplicationUser> Users { get; set; } = [];
}
