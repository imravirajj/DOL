using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class VehicleVariant : AuditableEntity, ITenantScoped
{
    public Guid CompanyId { get; private set; }
    public Guid VehicleModelId { get; private set; }
    public string VariantName { get; private set; } = string.Empty; // e.g. "Dark Edition SX AT"
    public string FuelType { get; private set; } = "Petrol";        // Petrol, Diesel, EV, Hybrid
    public string Transmission { get; private set; } = "Automatic"; // Manual, Automatic, DCT, EV
    public decimal ExShowroomPrice { get; private set; }
    public string ColorsAvailable { get; private set; } = string.Empty; // Comma-separated: "Black, White, Grey"
    public bool IsActive { get; private set; } = true;

    public VehicleModel? VehicleModel { get; private set; }

    private readonly List<VehicleStock> _stockUnits = new();
    public IReadOnlyCollection<VehicleStock> StockUnits => _stockUnits.AsReadOnly();

    private VehicleVariant() { } // EF Core

    public VehicleVariant(
        Guid companyId,
        Guid vehicleModelId,
        string variantName,
        string fuelType,
        string transmission,
        decimal exShowroomPrice,
        string colorsAvailable)
    {
        CompanyId = companyId;
        VehicleModelId = vehicleModelId;
        VariantName = variantName.Trim();
        FuelType = fuelType.Trim();
        Transmission = transmission.Trim();
        ExShowroomPrice = exShowroomPrice;
        ColorsAvailable = colorsAvailable.Trim();
        IsActive = true;
    }

    public void UpdateDetails(string variantName, string fuelType, string transmission, decimal exShowroomPrice, string colorsAvailable)
    {
        VariantName = variantName.Trim();
        FuelType = fuelType.Trim();
        Transmission = transmission.Trim();
        ExShowroomPrice = exShowroomPrice;
        ColorsAvailable = colorsAvailable.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
