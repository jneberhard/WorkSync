namespace WorkSync.Models;

public class FollowUpItem
{
    public int Id { get; set; }

    public DateTime MeetingDate { get; set; } = DateTime.Today;

    public string FollowUpItemTitle { get; set; } = "";

    public string AssignedLeader { get; set; } = "";

    public string RelatedFamily { get; set; } = "";

    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

    public string Status { get; set; } = "Open";

    public bool PrivacyFlag { get; set; }

    public string CompletionNotes { get; set; } = "";
}
