using DOL.SharedKernel;

namespace DOL.Identity.Domain.Events;

public class BranchCreatedEvent : IDomainEvent
{
    public Guid BranchId { get; }
    public Guid CompanyId { get; }
    public string BranchName { get; }
    public string BranchCode { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public BranchCreatedEvent(Guid branchId, Guid companyId, string branchName, string branchCode)
    {
        BranchId = branchId;
        CompanyId = companyId;
        BranchName = branchName;
        BranchCode = branchCode;
    }
}
