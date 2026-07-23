using System.ComponentModel.DataAnnotations;

namespace WorkSync.Models;

public class Workorder
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Workorder title is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Department is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Department must be between 2 and 100 characters.")]
    public string Department { get; set; } = "";

    [Required(ErrorMessage = "Assigned employee is required.")]
    [StringLength(100, ErrorMessage = "Employee name cannot exceed 100 characters.")]
    [RegularExpression(@"^[a-zA-Z\s\-\.']+$", ErrorMessage = "Employee name can only contain letters, spaces, hyphens, dots, and apostrophes.")]
    public string AssignedEmployee { get; set; } = "";

    [StringLength(20)]
    public string Status { get; set; } = "Open";

    public bool NeedsFollowUp { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
    public string Notes { get; set; } = "";
}
