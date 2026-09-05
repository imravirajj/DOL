using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class WarrantyPackage : AuditableEntity, ITenantScoped
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty; // e.g. "Shield of Trust 5-Year Extended Warranty"
    public WarrantyPackageType PackageType { get; private set; } = WarrantyPackageType.ExtendedWarranty;
    public int DurationMonths { get; private set; } = 36;
    public int KilometerLimit { get; private set; } = 100000;
    public decimal Price { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private WarrantyPackage() { } // EF Core

    public WarrantyPackage(
        Guid companyId,
        string name,
        WarrantyPackageType packageType,
        int durationMonths,
        int kilometerLimit,
        decimal price,
        string description)
    {
        CompanyId = companyId;
        Name = name.Trim();
        PackageType = packageType;
        DurationMonths = durationMonths;
        KilometerLimit = kilometerLimit;
        Price = price;
        Description = description.Trim();
        IsActive = true;
    }

    public void Update(string name, WarrantyPackageType packageType, int durationMonths, int kilometerLimit, decimal price, string description, bool isActive)
    {
        Name = name.Trim();
        PackageType = packageType;
        DurationMonths = durationMonths;
        KilometerLimit = kilometerLimit;
        Price = price;
        Description = description.Trim();
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
