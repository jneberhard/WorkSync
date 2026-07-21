using System.Text;
using WorkSync.Models;

namespace WorkSync.Services;

public class CalendarExportService
{
    // Creates an iCalendar (.ics) file for a selected assignment
    public string CreateAssignmentCalendarFile(AssignmentItem assignment)
    {
        // Calendar event dates
        var startDate = assignment.DueDate.Date;
        var endDate = startDate.AddDays(1);

        // Event title displayed in the user's calendar
        var title = EscapeText($"WorkSync Assignment: {assignment.Title}");

        // Event description containing assignment details
        var description = EscapeText(
            $"Description: {assignment.Description}\\n" +
            $"Assigned Leader: {assignment.AssignedLeader}\\n" +
            $"Organization: {assignment.Organization}\\n" +
            $"Priority: {assignment.Priority}\\n" +
            $"Status: {assignment.Status}\\n" +
            $"Notes: {assignment.Notes}"
        );

        // Build the iCalendar file content
        var builder = new StringBuilder();

        builder.AppendLine("BEGIN:VCALENDAR");
        builder.AppendLine("VERSION:2.0");
        builder.AppendLine("PRODID:-//WorkSync//Assignment Calendar//EN");

        builder.AppendLine("BEGIN:VEVENT");

        // Unique identifier for the calendar event
        builder.AppendLine($"UID:worksync-assignment-{assignment.Id}@worksync");

        // Timestamp indicating when the event was created
        builder.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");

        // Assignment due date shown as an all-day event
        builder.AppendLine($"DTSTART;VALUE=DATE:{startDate:yyyyMMdd}");
        builder.AppendLine($"DTEND;VALUE=DATE:{endDate:yyyyMMdd}");

        builder.AppendLine($"SUMMARY:{title}");
        builder.AppendLine($"DESCRIPTION:{description}");

        builder.AppendLine("END:VEVENT");
        builder.AppendLine("END:VCALENDAR");

        return builder.ToString();
    }

    // Escapes special characters required by the iCalendar format
    private static string EscapeText(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace(",", "\\,")
            .Replace(";", "\\;")
            .Replace("\n", "\\n")
            .Replace("\r", "");
    }
}