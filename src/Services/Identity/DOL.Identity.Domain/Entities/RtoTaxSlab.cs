using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class RtoTaxSlab : BaseEntity, ITenantScoped
{
    public Guid CompanyId { get; private set; }
    public string StateName { get; private set; } = string.Empty;   // e.g. "Maharashtra", "Delhi", "Karnataka"
    public string FuelType { get; private set; } = string.Empty;    // "Petrol", "Diesel", "EV", "Hybrid"
    public decimal TaxPercentage { get; private set; }              // e.g. 10.00%
    public decimal CessPercentage { get; private set; } = 0;        // e.g. 1.00%

    private RtoTaxSlab() { } // EF Core

    public RtoTaxSlab(Guid companyId, string stateName, string fuelType, decimal taxPercentage, decimal cessPercentage = 0)
    {
        CompanyId = companyId;
        StateName = stateName.Trim();
        FuelType = fuelType.Trim();
        TaxPercentage = taxPercentage;
        CessPercentage = cessPercentage;
    }
}
