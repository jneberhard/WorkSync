using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using WorkSync.Data;
using WorkSync.Models;

namespace WorkSync.Services;

// Service class responsible for handling all database operations
// related to Workorders, Assignments, Follow-Ups, and Members.
public class WorkSyncService(
    IDbContextFactory<ApplicationDbContext> dbFactory,
    AuthenticationStateProvider authenticationStateProvider,
    IHttpContextAccessor httpContextAccessor) : IDisposable
{
    private readonly ApplicationDbContext db = dbFactory.CreateDbContext();

    private async Task<int> GetTenantIdAsync()
    {
        var user = httpContextAccessor.HttpContext?.User ??
            (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        var tenantClaim = user.FindFirst("tenant_id")?.Value;

        if (!int.TryParse(tenantClaim, out var tenantId))
        {
            throw new UnauthorizedAccessException("A tenant is required for this operation.");
        }

        return tenantId;
    }

    public void Dispose() => db.Dispose();

    // =========================
    // WORKORDERS CRUD
    // =========================

    // Retrieve all workorders from the database
    public async Task<List<Workorder>> GetWorkordersAsync()
    {
        var tenantId = await GetTenantIdAsync();
        return await db.Workorders.Where(item => item.TenantId == tenantId).ToListAsync();
    }

    // Add a new workorder
    public async Task AddWorkorderAsync(Workorder workorder)
    {
        workorder.TenantId = await GetTenantIdAsync();
        db.Workorders.Add(workorder);
        await db.SaveChangesAsync();
    }

    // Update an existing workorder
    public async Task UpdateWorkorderAsync(Workorder workorder)
    {
        var tenantId = await GetTenantIdAsync();
        var existingWorkorder = await db.Workorders
            .SingleOrDefaultAsync(item => item.Id == workorder.Id && item.TenantId == tenantId);

        if (existingWorkorder != null)
        {
            existingWorkorder.Title = workorder.Title;
            existingWorkorder.Department = workorder.Department;
            existingWorkorder.AssignedEmployee = workorder.AssignedEmployee;
            existingWorkorder.Status = workorder.Status;
            existingWorkorder.NeedsFollowUp = workorder.NeedsFollowUp;
            existingWorkorder.CreatedDate = workorder.CreatedDate;
            existingWorkorder.DueDate = workorder.DueDate;
            existingWorkorder.CompletedDate = workorder.CompletedDate;
            existingWorkorder.Notes = workorder.Notes;

            await db.SaveChangesAsync();
        }
    }

    // Delete a workorder by ID
    public async Task DeleteWorkorderAsync(int id)
    {
        var tenantId = await GetTenantIdAsync();
        var workorder = await db.Workorders
            .SingleOrDefaultAsync(item => item.Id == id && item.TenantId == tenantId);

        if (workorder != null)
        {
            db.Workorders.Remove(workorder);
            await db.SaveChangesAsync();
        }
    }


    // =========================
    // ASSIGNMENTS CRUD
    // =========================

    // Retrieve all assignments
    public async Task<List<AssignmentItem>> GetAssignmentsAsync()
    {
        var tenantId = await GetTenantIdAsync();
        return await db.Assignments.Where(item => item.TenantId == tenantId).ToListAsync();
    }

    // Retrieve a single assignment by ID
    public async Task<AssignmentItem?> GetAssignmentByIdAsync(int id)
    {
        var tenantId = await GetTenantIdAsync();
        return await db.Assignments
            .SingleOrDefaultAsync(item => item.Id == id && item.TenantId == tenantId);
    }

    // Add a new assignment
    public async Task AddAssignmentAsync(AssignmentItem assignment)
    {
        assignment.TenantId = await GetTenantIdAsync();
        db.Assignments.Add(assignment);
        await db.SaveChangesAsync();
    }

    // Update an existing assignment
    public async Task UpdateAssignmentAsync(AssignmentItem assignment)
    {
        var tenantId = await GetTenantIdAsync();
        var existingAssignment = await db.Assignments
            .SingleOrDefaultAsync(item => item.Id == assignment.Id && item.TenantId == tenantId);

        if (existingAssignment != null)
        {
            existingAssignment.Title = assignment.Title;
            existingAssignment.Description = assignment.Description;
            existingAssignment.AssignedOwner = assignment.AssignedOwner;
            existingAssignment.Department = assignment.Department;
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
        var tenantId = await GetTenantIdAsync();
        var assignment = await db.Assignments
            .SingleOrDefaultAsync(item => item.Id == id && item.TenantId == tenantId);

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
    public async Task<List<FollowUpItem>> GetFollowUpsAsync()
    {
        var tenantId = await GetTenantIdAsync();
        return await db.FollowUpItems.Where(item => item.TenantId == tenantId).ToListAsync();
    }

    // Retrieve a follow-up item by ID
    public async Task<FollowUpItem?> GetFollowUpByIdAsync(int id)
    {
        var tenantId = await GetTenantIdAsync();
        return await db.FollowUpItems
            .SingleOrDefaultAsync(item => item.Id == id && item.TenantId == tenantId);
    }

    // Add a new follow-up item
    public async Task AddFollowUpAsync(FollowUpItem followUp)
    {
        followUp.TenantId = await GetTenantIdAsync();
        db.FollowUpItems.Add(followUp);
        await db.SaveChangesAsync();
    }

    // Update an existing follow-up item
    public async Task UpdateFollowUpAsync(FollowUpItem followUp)
    {
        var tenantId = await GetTenantIdAsync();
        var existingFollowUp = await db.FollowUpItems
            .SingleOrDefaultAsync(item => item.Id == followUp.Id && item.TenantId == tenantId);

        if (existingFollowUp != null)
        {
            existingFollowUp.MeetingDate = followUp.MeetingDate;
            existingFollowUp.FollowUpItemTitle = followUp.FollowUpItemTitle;
            existingFollowUp.AssignedOwner = followUp.AssignedOwner;
            existingFollowUp.RelatedProject = followUp.RelatedProject;
            existingFollowUp.DueDate = followUp.DueDate;
            existingFollowUp.Status = followUp.Status;
            existingFollowUp.IsConfidential = followUp.IsConfidential;
            existingFollowUp.CompletionNotes = followUp.CompletionNotes;

            await db.SaveChangesAsync();
        }
    }

    // Delete a follow-up item by ID
    public async Task DeleteFollowUpAsync(int id)
    {
        var tenantId = await GetTenantIdAsync();
        var followUp = await db.FollowUpItems
            .SingleOrDefaultAsync(item => item.Id == id && item.TenantId == tenantId);

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
    public async Task<List<Member>> GetMembersAsync()
    {
        var tenantId = await GetTenantIdAsync();
        return await db.Members.Where(item => item.TenantId == tenantId).ToListAsync();
    }

    // Retrieve a single work member by ID
    public async Task<Member?> GetMemberByIdAsync(int id)
    {
        var tenantId = await GetTenantIdAsync();
        return await db.Members
            .SingleOrDefaultAsync(item => item.Id == id && item.TenantId == tenantId);
    }

    // Add a new work member
    public async Task AddMemberAsync(Member member)
    {
        member.TenantId = await GetTenantIdAsync();
        db.Members.Add(member);
        await db.SaveChangesAsync();
    }

    // Update an existing work member
    public async Task UpdateMemberAsync(Member member)
    {
        var tenantId = await GetTenantIdAsync();
        var existingMember = await db.Members
            .SingleOrDefaultAsync(item => item.Id == member.Id && item.TenantId == tenantId);

        if (existingMember != null)
        {
            existingMember.FirstName = member.FirstName;
            existingMember.LastName = member.LastName;
            existingMember.Email = member.Email;
            existingMember.Phone = member.Phone;
            existingMember.Department = member.Department;
            existingMember.CurrentWorkorder = member.CurrentWorkorder;
            existingMember.AvailabilityNotes = member.AvailabilityNotes;
            existingMember.ActiveStatus = member.ActiveStatus;

            await db.SaveChangesAsync();
        }
    }

    // Delete a work member by ID
    public async Task DeleteMemberAsync(int id)
    {
        var tenantId = await GetTenantIdAsync();
        var member = await db.Members
            .SingleOrDefaultAsync(item => item.Id == id && item.TenantId == tenantId);

        if (member != null)
        {
            db.Members.Remove(member);
            await db.SaveChangesAsync();
        }
    }
}
