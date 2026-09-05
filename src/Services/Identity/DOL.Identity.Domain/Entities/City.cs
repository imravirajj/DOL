using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class City : BaseEntity, ITenantScoped
{
    public Guid CompanyId { get; private set; }
    public Guid StateRegionId { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public StateRegion? StateRegion { get; private set; }

    private readonly List<Branch> _branches = new();
    public IReadOnlyCollection<Branch> Branches => _branches.AsReadOnly();

    private City() { } // EF Core

    public City(Guid companyId, Guid stateRegionId, string name)
    {
        CompanyId = companyId;
        StateRegionId = stateRegionId;
        Name = name.Trim();
    }
}
