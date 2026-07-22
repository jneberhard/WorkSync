using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkSync.Migrations
{
    /// <inheritdoc />
    public partial class CompanyMeetingWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Organization",
                table: "Workorders",
                newName: "Department");

            migrationBuilder.RenameColumn(
                name: "MemberAssigned",
                table: "Workorders",
                newName: "AssignedEmployee");

            migrationBuilder.RenameColumn(
                name: "DateSustained",
                table: "Workorders",
                newName: "DueDate");

            migrationBuilder.RenameColumn(
                name: "DateReleased",
                table: "Workorders",
                newName: "CompletedDate");

            migrationBuilder.RenameColumn(
                name: "DateExtended",
                table: "Workorders",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "Organization",
                table: "Members",
                newName: "Department");

            migrationBuilder.RenameColumn(
                name: "RelatedFamily",
                table: "FollowUpItems",
                newName: "RelatedProject");

            migrationBuilder.RenameColumn(
                name: "PrivacyFlag",
                table: "FollowUpItems",
                newName: "IsConfidential");

            migrationBuilder.RenameColumn(
                name: "AssignedLeader",
                table: "FollowUpItems",
                newName: "AssignedOwner");

            migrationBuilder.RenameColumn(
                name: "Organization",
                table: "Assignments",
                newName: "Department");

            migrationBuilder.RenameColumn(
                name: "AssignedLeader",
                table: "Assignments",
                newName: "AssignedOwner");

            migrationBuilder.Sql(
                """UPDATE "Workorders" SET "Status" = 'In Progress' WHERE "Status" = 'Filled';""");
            migrationBuilder.Sql(
                """UPDATE "Workorders" SET "Status" = 'Completed' WHERE "Status" = 'Released';""");

            migrationBuilder.UpdateData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Department", "Description", "DueDate", "Notes", "Title" },
                values: new object[] { "Operations", "Compile the agenda, project updates, and department metrics.", new DateTime(2026, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Confirm presenters and circulate the agenda one day early.", "Prepare quarterly planning meeting" });

            migrationBuilder.UpdateData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Department", "Description", "DueDate", "Status", "Title" },
                values: new object[] { "Facilities", "Compare pricing, delivery schedules, and support terms.", new DateTime(2026, 8, 3, 0, 0, 0, 0, DateTimeKind.Utc), "In Progress", "Review vendor proposals" });

            migrationBuilder.UpdateData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CompletionDate", "Department", "Description", "DueDate", "Title" },
                values: new object[] { new DateTime(2026, 7, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Customer Success", "Send the approved decisions, owners, and due dates to the team.", new DateTime(2026, 7, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Publish meeting action summary" });

            migrationBuilder.UpdateData(
                table: "FollowUpItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionNotes", "DueDate", "FollowUpItemTitle", "MeetingDate", "RelatedProject" },
                values: new object[] { "Confirm owners for each approved initiative.", new DateTime(2026, 7, 31, 0, 0, 0, 0, DateTimeKind.Utc), "Quarterly planning action items", new DateTime(2026, 7, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Q3 Operations Plan" });

            migrationBuilder.UpdateData(
                table: "FollowUpItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CompletionNotes", "DueDate", "FollowUpItemTitle", "MeetingDate", "RelatedProject", "Status" },
                values: new object[] { "Document access-control decisions before implementation.", new DateTime(2026, 7, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Security review follow-up", new DateTime(2026, 7, 18, 0, 0, 0, 0, DateTimeKind.Utc), "ERP Upgrade", "In Progress" });

            migrationBuilder.UpdateData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AvailabilityNotes", "CurrentWorkorder", "Department" },
                values: new object[] { "Available weekday mornings", "Equipment safety inspection", "Facilities" });

            migrationBuilder.UpdateData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AvailabilityNotes", "CurrentWorkorder", "Department" },
                values: new object[] { "Available afternoons", "Quarterly inventory review", "Operations" });

            migrationBuilder.UpdateData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AvailabilityNotes", "CurrentWorkorder", "Department" },
                values: new object[] { "Available for customer meetings after 10:00 AM", "Client onboarding checklist", "Customer Success" });

            migrationBuilder.UpdateData(
                table: "Workorders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "Department", "DueDate", "Notes", "Title" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Facilities", new DateTime(2026, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Review the inspection checklist at the next operations meeting.", "Equipment safety inspection" });

            migrationBuilder.UpdateData(
                table: "Workorders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Department", "DueDate", "NeedsFollowUp", "Notes", "Status", "Title" },
                values: new object[] { new DateTime(2026, 7, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", new DateTime(2026, 8, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, "Confirm outstanding counts with warehouse leads.", "In Progress", "Quarterly inventory review" });

            migrationBuilder.UpdateData(
                table: "Workorders",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AssignedEmployee", "CompletedDate", "CreatedDate", "Department", "DueDate", "NeedsFollowUp", "Notes", "Status", "Title" },
                values: new object[] { "Tom Rivera", new DateTime(2026, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Customer Success", new DateTime(2026, 7, 18, 0, 0, 0, 0, DateTimeKind.Utc), false, "Access, training, and kickoff steps completed.", "Completed", "Client onboarding checklist" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """UPDATE "Workorders" SET "Status" = 'Filled' WHERE "Status" = 'In Progress';""");
            migrationBuilder.Sql(
                """UPDATE "Workorders" SET "Status" = 'Released' WHERE "Status" = 'Completed';""");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Workorders",
                newName: "DateSustained");

            migrationBuilder.RenameColumn(
                name: "Department",
                table: "Workorders",
                newName: "Organization");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Workorders",
                newName: "DateExtended");

            migrationBuilder.RenameColumn(
                name: "CompletedDate",
                table: "Workorders",
                newName: "DateReleased");

            migrationBuilder.RenameColumn(
                name: "AssignedEmployee",
                table: "Workorders",
                newName: "MemberAssigned");

            migrationBuilder.RenameColumn(
                name: "Department",
                table: "Members",
                newName: "Organization");

            migrationBuilder.RenameColumn(
                name: "RelatedProject",
                table: "FollowUpItems",
                newName: "RelatedFamily");

            migrationBuilder.RenameColumn(
                name: "IsConfidential",
                table: "FollowUpItems",
                newName: "PrivacyFlag");

            migrationBuilder.RenameColumn(
                name: "AssignedOwner",
                table: "FollowUpItems",
                newName: "AssignedLeader");

            migrationBuilder.RenameColumn(
                name: "Department",
                table: "Assignments",
                newName: "Organization");

            migrationBuilder.RenameColumn(
                name: "AssignedOwner",
                table: "Assignments",
                newName: "AssignedLeader");

            migrationBuilder.UpdateData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "DueDate", "Notes", "Organization", "Title" },
                values: new object[] { "Plan transport and refreshments for youth activity.", new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Confirm van availability.", "Young Men", "Youth Activity Planning" });

            migrationBuilder.UpdateData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "DueDate", "Organization", "Status", "Title" },
                values: new object[] { "Follow up with sisters who haven't been contacted.", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Relief Society", "Open", "Ministering Interview Follow-Up" });

            migrationBuilder.UpdateData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CompletionDate", "Description", "DueDate", "Organization", "Title" },
                values: new object[] { new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Update weekly bulletin with new announcements.", new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Bishopric", "Bulletin Update" });

            migrationBuilder.UpdateData(
                table: "FollowUpItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionNotes", "DueDate", "FollowUpItemTitle", "MeetingDate", "RelatedFamily" },
                values: new object[] { "", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Work Council Action Item", new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Sample Family" });

            migrationBuilder.UpdateData(
                table: "FollowUpItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CompletionNotes", "DueDate", "FollowUpItemTitle", "MeetingDate", "RelatedFamily", "Status" },
                values: new object[] { "", new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Service Coordination", new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Sample Family B", "Open" });

            migrationBuilder.UpdateData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AvailabilityNotes", "CurrentWorkorder", "Organization" },
                values: new object[] { "Evenings preferred", "Relief Society President", "Relief Society" });

            migrationBuilder.UpdateData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AvailabilityNotes", "CurrentWorkorder", "Organization" },
                values: new object[] { "Available weekends", "Elders Quorum President", "Elders Quorum" });

            migrationBuilder.UpdateData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AvailabilityNotes", "CurrentWorkorder", "Organization" },
                values: new object[] { "", "First Counselor", "Bishopric" });

            migrationBuilder.UpdateData(
                table: "Workorders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateReleased", "DateSustained", "Notes", "Organization", "Title" },
                values: new object[] { null, null, "Needs to be filled before next month.", "Primary", "Primary Teacher" });

            migrationBuilder.UpdateData(
                table: "Workorders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateReleased", "DateSustained", "NeedsFollowUp", "Notes", "Organization", "Status", "Title" },
                values: new object[] { null, new DateTime(2024, 1, 14, 0, 0, 0, 0, DateTimeKind.Utc), false, "", "Elders Quorum", "Filled", "Elders Quorum President" });

            migrationBuilder.UpdateData(
                table: "Workorders",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateExtended", "DateReleased", "DateSustained", "MemberAssigned", "NeedsFollowUp", "Notes", "Organization", "Status", "Title" },
                values: new object[] { null, null, null, "", true, "", "Relief Society", "Open", "Relief Society Secretary" });
        }
    }
}
