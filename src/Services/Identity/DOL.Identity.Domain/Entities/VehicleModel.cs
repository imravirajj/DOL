using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class VehicleModel : AuditableEntity, ITenantScoped
{
    public Guid CompanyId { get; private set; }
    public string Make { get; private set; } = string.Empty;   // e.g. "Tata", "Hyundai"
    public string Model { get; private set; } = string.Empty;  // e.g. "Harrier", "Creta"
    public int Year { get; private set; } = 2026;
    public string Category { get; private set; } = "SUV";      // SUV, Sedan, Hatchback, EV
    public bool IsActive { get; private set; } = true;

    private readonly List<VehicleVariant> _variants = new();
    public IReadOnlyCollection<VehicleVariant> Variants => _variants.AsReadOnly();

    private VehicleModel() { } // EF Core

    public VehicleModel(Guid companyId, string make, string model, int year, string category = "SUV")
    {
        CompanyId = companyId;
        Make = make.Trim();
        Model = model.Trim();
        Year = year;
        Category = category.Trim();
        IsActive = true;
    }

    public void AddVariant(VehicleVariant variant)
    {
        _variants.Add(variant);
    }
}
