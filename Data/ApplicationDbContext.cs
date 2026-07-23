using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkSync.Models;

namespace WorkSync.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IDataProtectionKeyContext
{
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();
    public DbSet<Workorder> Workorders => Set<Workorder>();
    public DbSet<AssignmentItem> Assignments => Set<AssignmentItem>();
    public DbSet<FollowUpItem> FollowUpItems => Set<FollowUpItem>();
    public DbSet<Member> Members => Set<Member>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Workorder>().HasData(
            new Workorder
            {
                Id = 1,
                Title = "Equipment safety inspection",
                Department = "Facilities",
                AssignedEmployee = "Sarah Johnson",
                Status = "Open",
                NeedsFollowUp = true,
                CreatedDate = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                Notes = "Review the inspection checklist at the next operations meeting."
            },
            new Workorder
            {
                Id = 2,
                Title = "Quarterly inventory review",
                Department = "Operations",
                AssignedEmployee = "James Carter",
                Status = "In Progress",
                NeedsFollowUp = true,
                CreatedDate = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc),
                Notes = "Confirm outstanding counts with warehouse leads."
            },
            new Workorder
            {
                Id = 3,
                Title = "Client onboarding checklist",
                Department = "Customer Success",
                AssignedEmployee = "Tom Rivera",
                Status = "Completed",
                NeedsFollowUp = false,
                CreatedDate = new DateTime(2026, 7, 8, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc),
                CompletedDate = new DateTime(2026, 7, 17, 0, 0, 0, DateTimeKind.Utc),
                Notes = "Access, training, and kickoff steps completed."
            }
        );

        builder.Entity<AssignmentItem>().HasData(
            new AssignmentItem
            {
                Id = 1,
                Title = "Prepare quarterly planning meeting",
                Description = "Compile the agenda, project updates, and department metrics.",
                AssignedOwner = "James Carter",
                Department = "Operations",
                DueDate = new DateTime(2026, 7, 28, 0, 0, 0, DateTimeKind.Utc),
                Priority = "High",
                Status = "Open",
                Notes = "Confirm presenters and circulate the agenda one day early."
            },
            new AssignmentItem
            {
                Id = 2,
                Title = "Review vendor proposals",
                Description = "Compare pricing, delivery schedules, and support terms.",
                AssignedOwner = "Sarah Johnson",
                Department = "Facilities",
                DueDate = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc),
                Priority = "Medium",
                Status = "In Progress"
            },
            new AssignmentItem
            {
                Id = 3,
                Title = "Publish meeting action summary",
                Description = "Send the approved decisions, owners, and due dates to the team.",
                AssignedOwner = "Tom Rivera",
                Department = "Customer Success",
                DueDate = new DateTime(2026, 7, 21, 0, 0, 0, DateTimeKind.Utc),
                Priority = "Low",
                Status = "Completed",
                CompletionDate = new DateTime(2026, 7, 21, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        builder.Entity<FollowUpItem>().HasData(
            new FollowUpItem
            {
                Id = 1,
                MeetingDate = new DateTime(2026, 7, 21, 0, 0, 0, DateTimeKind.Utc),
                FollowUpItemTitle = "Quarterly planning action items",
                AssignedOwner = "James Carter",
                RelatedProject = "Q3 Operations Plan",
                DueDate = new DateTime(2026, 7, 31, 0, 0, 0, DateTimeKind.Utc),
                Status = "Open",
                IsConfidential = false,
                CompletionNotes = "Confirm owners for each approved initiative."
            },
            new FollowUpItem
            {
                Id = 2,
                MeetingDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc),
                FollowUpItemTitle = "Security review follow-up",
                AssignedOwner = "Sarah Johnson",
                RelatedProject = "ERP Upgrade",
                DueDate = new DateTime(2026, 7, 25, 0, 0, 0, DateTimeKind.Utc),
                Status = "In Progress",
                IsConfidential = true,
                CompletionNotes = "Document access-control decisions before implementation."
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
                Department = "Facilities",
                CurrentWorkorder = "Equipment safety inspection",
                AvailabilityNotes = "Available weekday mornings",
                ActiveStatus = true
            },
            new Member
            {
                Id = 2,
                FirstName = "James",
                LastName = "Carter",
                Email = "james@example.com",
                Phone = "555-2222",
                Department = "Operations",
                CurrentWorkorder = "Quarterly inventory review",
                AvailabilityNotes = "Available afternoons",
                ActiveStatus = true
            },
            new Member
            {
                Id = 3,
                FirstName = "Tom",
                LastName = "Rivera",
                Email = "tom@example.com",
                Phone = "555-3333",
                Department = "Customer Success",
                CurrentWorkorder = "Client onboarding checklist",
                AvailabilityNotes = "Available for customer meetings after 10:00 AM",
                ActiveStatus = true
            }
        );
    }
}
