using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class VehicleAccessory : AuditableEntity, ITenantScoped
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string PartNumber { get; private set; } = string.Empty;
    public AccessoryCategory Category { get; private set; } = AccessoryCategory.Exterior;
    public Guid? CompatibleVariantId { get; private set; }
    public decimal Price { get; private set; }
    public decimal InstallationCost { get; private set; }
    public int WarrantyMonths { get; private set; } = 12;
    public bool IsActive { get; private set; } = true;

    public VehicleVariant? CompatibleVariant { get; private set; }

    private VehicleAccessory() { } // EF Core

    public VehicleAccessory(
        Guid companyId,
        string name,
        string partNumber,
        AccessoryCategory category,
        decimal price,
        decimal installationCost = 0,
        int warrantyMonths = 12,
        Guid? compatibleVariantId = null)
    {
        CompanyId = companyId;
        Name = name.Trim();
        PartNumber = partNumber.Trim().ToUpperInvariant();
        Category = category;
        Price = price;
        InstallationCost = installationCost;
        WarrantyMonths = warrantyMonths;
        CompatibleVariantId = compatibleVariantId;
        IsActive = true;
    }

    public void Update(
        string name,
        AccessoryCategory category,
        decimal price,
        decimal installationCost,
        int warrantyMonths,
        Guid? compatibleVariantId,
        bool isActive)
    {
        Name = name.Trim();
        Category = category;
        Price = price;
        InstallationCost = installationCost;
        WarrantyMonths = warrantyMonths;
        CompatibleVariantId = compatibleVariantId;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
