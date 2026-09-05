using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class StateRegion : BaseEntity, ITenantScoped
{
    public Guid CompanyId { get; private set; }
    public Guid CountryId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? StateCode { get; private set; }

    public Country? Country { get; private set; }

    private readonly List<City> _cities = new();
    public IReadOnlyCollection<City> Cities => _cities.AsReadOnly();

    private StateRegion() { } // EF Core

    public StateRegion(Guid companyId, Guid countryId, string name, string? stateCode = null)
    {
        CompanyId = companyId;
        CountryId = countryId;
        Name = name.Trim();
        StateCode = stateCode?.Trim().ToUpperInvariant();
    }
}
