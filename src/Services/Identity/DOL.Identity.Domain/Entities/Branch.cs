using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class Branch : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId => Id;
    public Guid CityId { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string BranchCode { get; private set; } = string.Empty; // e.g. "MUM-AND-01"
    public string Address { get; private set; } = string.Empty;
    public string? ContactPhone { get; private set; }
    public string? ContactEmail { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsMainBranch { get; private set; } = false;

    public Company? Company { get; private set; }
    public City? City { get; private set; }

    private readonly List<ApplicationUser> _users = new();
    public IReadOnlyCollection<ApplicationUser> Users => _users.AsReadOnly();

    private Branch() { } // EF Core

    public Branch(
        Guid companyId,
        Guid cityId,
        string name,
        string branchCode,
        string address,
        string? contactPhone = null,
        string? contactEmail = null,
        bool isMainBranch = false)
    {
        CompanyId = companyId;
        CityId = cityId;
        Name = name.Trim();
        BranchCode = branchCode.Trim().ToUpperInvariant();
        Address = address.Trim();
        ContactPhone = contactPhone?.Trim();
        ContactEmail = contactEmail?.Trim().ToLowerInvariant();
        IsActive = true;
        IsMainBranch = isMainBranch;
    }

    public void UpdateDetails(string name, string address, string? contactPhone, string? contactEmail)
    {
        Name = name.Trim();
        Address = address.Trim();
        ContactPhone = contactPhone?.Trim();
        ContactEmail = contactEmail?.Trim().ToLowerInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
