using DOL.SharedKernel;

namespace DOL.Identity.Domain.Events;

public class CompanyRegisteredEvent : IDomainEvent
{
    public Guid CompanyId { get; }
    public string CompanyName { get; }
    public string CompanyCode { get; }
    public Guid AdminUserId { get; }
    public string AdminEmail { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public CompanyRegisteredEvent(Guid companyId, string companyName, string companyCode, Guid adminUserId, string adminEmail)
    {
        CompanyId = companyId;
        CompanyName = companyName;
        CompanyCode = companyCode;
        AdminUserId = adminUserId;
        AdminEmail = adminEmail;
    }
}
