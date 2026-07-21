using Microsoft.EntityFrameworkCore;
using WorkSync.Data;
using WorkSync.Models;

namespace WorkSync.Services;

// Service class responsible for handling all database operations
// related to Callings, Assignments, Follow-Ups, and Members.
public class WorkSyncService(ApplicationDbContext db)
{
    // =========================
    // CALLINGS CRUD
    // =========================

    // Retrieve all callings from the database
    public Task<List<Calling>> GetCallingsAsync() =>
        db.Callings.ToListAsync();

    // Add a new calling
    public async Task AddCallingAsync(Calling calling)
    {
        db.Callings.Add(calling);
        await db.SaveChangesAsync();
    }

    // Update an existing calling
    public async Task UpdateCallingAsync(Calling calling)
    {
        var existingCalling = await db.Callings.FindAsync(calling.Id);

        if (existingCalling != null)
        {
            existingCalling.Title = calling.Title;
            existingCalling.Organization = calling.Organization;
            existingCalling.MemberAssigned = calling.MemberAssigned;
            existingCalling.Status = calling.Status;
            existingCalling.NeedsFollowUp = calling.NeedsFollowUp;
            existingCalling.DateExtended = calling.DateExtended;
            existingCalling.DateSustained = calling.DateSustained;
            existingCalling.DateReleased = calling.DateReleased;
            existingCalling.Notes = calling.Notes;

            await db.SaveChangesAsync();
        }
    }

    // Delete a calling by ID
    public async Task DeleteCallingAsync(int id)
    {
        var calling = await db.Callings.FindAsync(id);

        if (calling != null)
        {
            db.Callings.Remove(calling);
            await db.SaveChangesAsync();
        }
    }


    // =========================
    // ASSIGNMENTS CRUD
    // =========================

    // Retrieve all assignments
    public Task<List<AssignmentItem>> GetAssignmentsAsync() =>
        db.Assignments.ToListAsync();

    // Retrieve a single assignment by ID
    public async Task<AssignmentItem?> GetAssignmentByIdAsync(int id)
    {
        return await db.Assignments.FindAsync(id);
    }

    // Add a new assignment
    public async Task AddAssignmentAsync(AssignmentItem assignment)
    {
        db.Assignments.Add(assignment);
        await db.SaveChangesAsync();
    }

    // Update an existing assignment
    public async Task UpdateAssignmentAsync(AssignmentItem assignment)
    {
        var existingAssignment = await db.Assignments.FindAsync(assignment.Id);

        if (existingAssignment != null)
        {
            existingAssignment.Title = assignment.Title;
            existingAssignment.Description = assignment.Description;
            existingAssignment.AssignedLeader = assignment.AssignedLeader;
            existingAssignment.Organization = assignment.Organization;
            existingAssignment.DueDate = assignment.DueDate;
            existingAssignment.Priority = assignment.Priority;
            existingAssignment.Status = assignment.Status;
            existingAssignment.Notes = assignment.Notes;
            existingAssignment.CompletionDate = assignment.CompletionDate;

            await db.SaveChangesAsync();
        }
    }

    // Delete an assignment by ID
    public async Task DeleteAssignmentAsync(int id)
    {
        var assignment = await db.Assignments.FindAsync(id);

        if (assignment != null)
        {
            db.Assignments.Remove(assignment);
            await db.SaveChangesAsync();
        }
    }

    // =========================
    // FOLLOW UPS CRUD
    // =========================

    // Retrieve all follow-up items
    public Task<List<FollowUpItem>> GetFollowUpsAsync() =>
        db.FollowUpItems.ToListAsync();

    // Retrieve a follow-up item by ID
    public async Task<FollowUpItem?> GetFollowUpByIdAsync(int id)
    {
        return await db.FollowUpItems.FindAsync(id);
    }

    // Add a new follow-up item
    public async Task AddFollowUpAsync(FollowUpItem followUp)
    {
        db.FollowUpItems.Add(followUp);
        await db.SaveChangesAsync();
    }

    // Update an existing follow-up item
    public async Task UpdateFollowUpAsync(FollowUpItem followUp)
    {
        var existingFollowUp = await db.FollowUpItems.FindAsync(followUp.Id);

        if (existingFollowUp != null)
        {
            existingFollowUp.MeetingDate = followUp.MeetingDate;
            existingFollowUp.FollowUpItemTitle = followUp.FollowUpItemTitle;
            existingFollowUp.AssignedLeader = followUp.AssignedLeader;
            existingFollowUp.RelatedFamily = followUp.RelatedFamily;
            existingFollowUp.DueDate = followUp.DueDate;
            existingFollowUp.Status = followUp.Status;
            existingFollowUp.PrivacyFlag = followUp.PrivacyFlag;
            existingFollowUp.CompletionNotes = followUp.CompletionNotes;

            await db.SaveChangesAsync();
        }
    }

    // Delete a follow-up item by ID
    public async Task DeleteFollowUpAsync(int id)
    {
        var followUp = await db.FollowUpItems.FindAsync(id);

        if (followUp != null)
        {
            db.FollowUpItems.Remove(followUp);
            await db.SaveChangesAsync();
        }
    }

    // =========================
    // MEMBERS
    // =========================

    // Retrieve all work members
    public Task<List<Member>> GetMembersAsync() =>
        db.Members.ToListAsync();

    // Retrieve a single work member by ID
    public Task<Member?> GetMemberByIdAsync(int id) =>
        db.Members.FindAsync(id).AsTask();

    // Add a new work member
    public async Task AddMemberAsync(Member member)
    {
        db.Members.Add(member);
        await db.SaveChangesAsync();
    }

    // Update an existing work member
    public async Task UpdateMemberAsync(Member member)
    {
        var existingMember = await db.Members.FindAsync(member.Id);

        if (existingMember != null)
        {
            existingMember.FirstName = member.FirstName;
            existingMember.LastName = member.LastName;
            existingMember.Email = member.Email;
            existingMember.Phone = member.Phone;
            existingMember.Organization = member.Organization;
            existingMember.CurrentCalling = member.CurrentCalling;
            existingMember.AvailabilityNotes = member.AvailabilityNotes;
            existingMember.ActiveStatus = member.ActiveStatus;

            await db.SaveChangesAsync();
        }
    }

    // Delete a work member by ID
    public async Task DeleteMemberAsync(int id)
    {
        var member = await db.Members.FindAsync(id);

        if (member != null)
        {
            db.Members.Remove(member);
            await db.SaveChangesAsync();
        }
    }
}
