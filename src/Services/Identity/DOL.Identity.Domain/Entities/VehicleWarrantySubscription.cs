using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class VehicleWarrantySubscription : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid? OrderId { get; private set; }
    public Guid WarrantyPackageId { get; private set; }

    public string VinNumber { get; private set; } = string.Empty;
    public string SubscriptionNumber { get; private set; } = string.Empty; // e.g. "WRN-2026-MUM-4401"
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal PricePaid { get; private set; }
    public WarrantyStatus Status { get; private set; } = WarrantyStatus.Active;

    public WarrantyPackage? WarrantyPackage { get; private set; }
    public VehicleOrder? Order { get; private set; }

    private VehicleWarrantySubscription() { } // EF Core

    public VehicleWarrantySubscription(
        Guid companyId,
        Guid branchId,
        Guid buyerId,
        Guid warrantyPackageId,
        string vinNumber,
        string subscriptionNumber,
        DateTime startDate,
        DateTime endDate,
        decimal pricePaid,
        Guid? orderId = null)
    {
        CompanyId = companyId;
        BranchId = branchId;
        BuyerId = buyerId;
        WarrantyPackageId = warrantyPackageId;
        VinNumber = vinNumber.Trim().ToUpperInvariant();
        SubscriptionNumber = subscriptionNumber.Trim().ToUpperInvariant();
        StartDate = startDate;
        EndDate = endDate;
        PricePaid = pricePaid;
        OrderId = orderId;
        Status = WarrantyStatus.Active;
    }

    public void Cancel()
    {
        Status = WarrantyStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
