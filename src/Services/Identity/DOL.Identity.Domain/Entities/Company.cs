using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class Company : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty; // Unique code e.g. "ACME-CORP"
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string? Address { get; private set; }
    public string SubscriptionPlan { get; private set; } = "Enterprise";
    public CompanyStatus Status { get; private set; } = CompanyStatus.Active;
    public string Currency { get; private set; } = "USD";
    public string TimeZone { get; private set; } = "UTC";

    private readonly List<Branch> _branches = new();
    public IReadOnlyCollection<Branch> Branches => _branches.AsReadOnly();

    private readonly List<ApplicationUser> _users = new();
    public IReadOnlyCollection<ApplicationUser> Users => _users.AsReadOnly();

    private Company() { } // EF Core

    public Company(
        string name,
        string code,
        string email,
        string phoneNumber,
        string? address = null,
        string subscriptionPlan = "Enterprise",
        string currency = "USD",
        string timeZone = "UTC")
    {
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Email = email.Trim().ToLowerInvariant();
        PhoneNumber = phoneNumber.Trim();
        Address = address;
        SubscriptionPlan = subscriptionPlan;
        Status = CompanyStatus.Active;
        Currency = currency;
        TimeZone = timeZone;
    }

    public void UpdateDetails(string name, string phoneNumber, string? address, string currency, string timeZone)
    {
        Name = name.Trim();
        PhoneNumber = phoneNumber.Trim();
        Address = address;
        Currency = currency;
        TimeZone = timeZone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(CompanyStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
