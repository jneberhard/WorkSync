using System.ComponentModel.DataAnnotations;

namespace WorkSync.Models;

public class AssignmentItem
{
    // Primary key for the assignment record
    public int Id { get; set; }

    // Title of the assignment or task
    [Required(ErrorMessage = "Assignment title is required.")]
    [StringLength(100)]
    public string Title { get; set; } = "";

    // Detailed description of the assignment
    [Required(ErrorMessage = "Description is required.")]
    [StringLength(500)]
    public string Description { get; set; } = "";

    // Leader responsible for completing the assignment
    [Required(ErrorMessage = "Assigned leader is required.")]
    [StringLength(100)]
    public string AssignedLeader { get; set; } = "";

    // Organization or auxiliary responsible for the assignment
    [Required(ErrorMessage = "Organization is required.")]
    [StringLength(100)]
    public string Organization { get; set; } = "";

    // Date by which the assignment should be completed
    [Required(ErrorMessage = "Due date is required.")]
    public DateTime DueDate { get; set; } = DateTime.Today;

    // Priority level: Low, Medium, or High
    [Required]
    public string Priority { get; set; } = "Medium";

    // Current assignment status: Open, In Progress, or Completed
    [Required]
    public string Status { get; set; } = "Open";

    // Additional notes or follow-up information
    [StringLength(1000)]
    public string Notes { get; set; } = "";

    // Date the assignment was completed (optional)
    public DateTime? CompletionDate { get; set; }
}