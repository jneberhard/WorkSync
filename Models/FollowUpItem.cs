namespace WorkSync.Models;

public class FollowUpItem
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    public DateTime MeetingDate { get; set; } = DateTime.Today;

    public string FollowUpItemTitle { get; set; } = "";

    public string AssignedOwner { get; set; } = "";

    public string RelatedProject { get; set; } = "";

    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

    public string Status { get; set; } = "Open";

    public bool IsConfidential { get; set; }

    public string CompletionNotes { get; set; } = "";
}
