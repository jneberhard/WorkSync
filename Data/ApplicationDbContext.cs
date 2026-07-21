using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkSync.Models;

namespace WorkSync.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Calling> Callings => Set<Calling>();
    public DbSet<AssignmentItem> Assignments => Set<AssignmentItem>();
    public DbSet<FollowUpItem> FollowUpItems => Set<FollowUpItem>();
    public DbSet<Member> Members => Set<Member>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Calling>().HasData(
            new Calling
            {
                Id = 1,
                Title = "Primary Teacher",
                Organization = "Primary",
                MemberAssigned = "Sarah Johnson",
                Status = "Open",
                NeedsFollowUp = true,
                Notes = "Needs to be filled before next month."
            },
            new Calling
            {
                Id = 2,
                Title = "Elders Quorum President",
                Organization = "Elders Quorum",
                MemberAssigned = "James Carter",
                Status = "Filled",
                NeedsFollowUp = false,
                DateSustained = new DateTime(2024, 1, 14)
            },
            new Calling
            {
                Id = 3,
                Title = "Relief Society Secretary",
                Organization = "Relief Society",
                MemberAssigned = "",
                Status = "Open",
                NeedsFollowUp = true
            }
        );

        builder.Entity<AssignmentItem>().HasData(
            new AssignmentItem
            {
                Id = 1,
                Title = "Youth Activity Planning",
                Description = "Plan transport and refreshments for youth activity.",
                AssignedLeader = "James Carter",
                Organization = "Young Men",
                DueDate = new DateTime(2026, 5, 27, 0, 0, 0, DateTimeKind.Local),
                Priority = "High",
                Status = "Open",
                Notes = "Confirm van availability."
            },
            new AssignmentItem
            {
                Id = 2,
                Title = "Ministering Interview Follow-Up",
                Description = "Follow up with sisters who haven't been contacted.",
                AssignedLeader = "Sarah Johnson",
                Organization = "Relief Society",
                DueDate = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Local),
                Priority = "Medium",
                Status = "Open"
            },
            new AssignmentItem
            {
                Id = 3,
                Title = "Bulletin Update",
                Description = "Update weekly bulletin with new announcements.",
                AssignedLeader = "Tom Rivera",
                Organization = "Bishopric",
                DueDate = new DateTime(2026, 5, 20, 0, 0, 0, DateTimeKind.Local),
                Priority = "Low",
                Status = "Completed",
                CompletionDate = new DateTime(2026, 5, 19, 0, 0, 0, DateTimeKind.Local)
            }
        );

        builder.Entity<FollowUpItem>().HasData(
            new FollowUpItem
            {
                Id = 1,
                MeetingDate = new DateTime(2026, 5, 23, 0, 0, 0, DateTimeKind.Local),
                FollowUpItemTitle = "Work Council Action Item",
                AssignedLeader = "James Carter",
                RelatedFamily = "Sample Family",
                DueDate = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Local),
                Status = "Open",
                PrivacyFlag = false,
                CompletionNotes = ""
            },
            new FollowUpItem
            {
                Id = 2,
                MeetingDate = new DateTime(2026, 5, 16, 0, 0, 0, DateTimeKind.Local),
                FollowUpItemTitle = "Service Coordination",
                AssignedLeader = "Sarah Johnson",
                RelatedFamily = "Sample Family B",
                DueDate = new DateTime(2026, 5, 28, 0, 0, 0, DateTimeKind.Local),
                Status = "Open",
                PrivacyFlag = true,
                CompletionNotes = ""
            }
        );

        builder.Entity<Member>().HasData(
            new Member
            {
                Id = 1,
                FirstName = "Sarah",
                LastName = "Johnson",
                Email = "sarah@example.com",
                Phone = "555-1111",
                Organization = "Relief Society",
                CurrentCalling = "Relief Society President",
                AvailabilityNotes = "Evenings preferred",
                ActiveStatus = true
            },
            new Member
            {
                Id = 2,
                FirstName = "James",
                LastName = "Carter",
                Email = "james@example.com",
                Phone = "555-2222",
                Organization = "Elders Quorum",
                CurrentCalling = "Elders Quorum President",
                AvailabilityNotes = "Available weekends",
                ActiveStatus = true
            },
            new Member
            {
                Id = 3,
                FirstName = "Tom",
                LastName = "Rivera",
                Email = "tom@example.com",
                Phone = "555-3333",
                Organization = "Bishopric",
                CurrentCalling = "First Counselor",
                AvailabilityNotes = "",
                ActiveStatus = true
            }
        );
    }
}
