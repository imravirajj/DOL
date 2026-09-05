using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class SalesLead : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid? AssignedStaffId { get; private set; }
    public Guid? InterestedModelId { get; private set; }

    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public string? CustomerEmail { get; private set; }
    public string LeadSource { get; private set; } = "Website"; // Website, WalkIn, SocialMedia, Referral
    public LeadPriority Priority { get; private set; } = LeadPriority.Hot;
    public LeadStage Stage { get; private set; } = LeadStage.New;

    public string? Notes { get; private set; }
    public DateTime? NextFollowUpDate { get; private set; }
    public string? LostReason { get; private set; }

    public ApplicationUser? AssignedStaff { get; private set; }
    public VehicleModel? InterestedModel { get; private set; }

    private SalesLead() { } // EF Core

    public SalesLead(
        Guid companyId,
        Guid branchId,
        string customerName,
        string customerPhone,
        string? customerEmail = null,
        string leadSource = "Website",
        LeadPriority priority = LeadPriority.Hot,
        Guid? interestedModelId = null,
        Guid? assignedStaffId = null,
        string? notes = null,
        DateTime? nextFollowUpDate = null)
    {
        CompanyId = companyId;
        BranchId = branchId;
        CustomerName = customerName.Trim();
        CustomerPhone = customerPhone.Trim();
        CustomerEmail = customerEmail?.Trim();
        LeadSource = leadSource.Trim();
        Priority = priority;
        InterestedModelId = interestedModelId;
        AssignedStaffId = assignedStaffId;
        Notes = notes?.Trim();
        NextFollowUpDate = nextFollowUpDate;
        Stage = LeadStage.New;
    }

    public void AssignStaff(Guid staffId)
    {
        AssignedStaffId = staffId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AdvanceStage(LeadStage newStage, string? lostReason = null)
    {
        Stage = newStage;
        if (newStage == LeadStage.Lost)
        {
            LostReason = lostReason?.Trim();
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void ScheduleFollowUp(DateTime followUpDate, string? notes = null)
    {
        NextFollowUpDate = followUpDate;
        if (!string.IsNullOrWhiteSpace(notes))
        {
            Notes = notes.Trim();
        }
        UpdatedAt = DateTime.UtcNow;
    }
}
