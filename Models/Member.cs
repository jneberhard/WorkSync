namespace WorkSync.Models;

public class Member
{
    public int Id { get; set; }

    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";

    public string Email { get; set; } = "";

    public string Phone { get; set; } = "";

    public string Organization { get; set; } = "";

    public string CurrentCalling { get; set; } = "";

    public string AvailabilityNotes { get; set; } = "";

    public bool ActiveStatus { get; set; } = true;

    public string FullName => $"{FirstName} {LastName}";
}