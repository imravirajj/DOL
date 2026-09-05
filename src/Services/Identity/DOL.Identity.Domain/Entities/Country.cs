using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class Country : BaseEntity, ITenantScoped
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string IsoCode { get; private set; } = string.Empty; // e.g. "IN", "US", "AE"

    private readonly List<StateRegion> _states = new();
    public IReadOnlyCollection<StateRegion> States => _states.AsReadOnly();

    private Country() { } // EF Core

    public Country(Guid companyId, string name, string isoCode)
    {
        CompanyId = companyId;
        Name = name.Trim();
        IsoCode = isoCode.Trim().ToUpperInvariant();
    }
}
