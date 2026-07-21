using System.ComponentModel.DataAnnotations;

namespace WorkSync.Models;

public class Calling
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Calling title is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Organization is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Organization must be between 2 and 100 characters.")]
    public string Organization { get; set; } = "";

    [Required(ErrorMessage = "Assigned member is required.")]
    [StringLength(100, ErrorMessage = "Member name cannot exceed 100 characters.")]
    [RegularExpression(@"^[a-zA-Z\s\-\.']+$", ErrorMessage = "Member name can only contain letters, spaces, hyphens, dots, and apostrophes.")]
    public string MemberAssigned { get; set; } = "";

    [StringLength(20)]
    public string Status { get; set; } = "Open";

    public bool NeedsFollowUp { get; set; }

    public DateTime? DateExtended { get; set; }

    public DateTime? DateSustained { get; set; }

    public DateTime? DateReleased { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
    public string Notes { get; set; } = "";
}