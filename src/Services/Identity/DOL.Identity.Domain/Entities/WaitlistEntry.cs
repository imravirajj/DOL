using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class WaitlistEntry : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid VehicleVariantId { get; private set; }
    public Guid BuyerId { get; private set; }

    public int QueuePosition { get; private set; } // Token #1, #2, #3
    public decimal TokenAmountPaid { get; private set; }
    public string IdempotencyKey { get; private set; } = string.Empty;
    public WaitlistStatus Status { get; private set; } = WaitlistStatus.Waiting;

    public Guid? AllocatedStockId { get; private set; }
    public DateTime? AllocatedAt { get; private set; }

    public Branch? Branch { get; private set; }
    public VehicleVariant? VehicleVariant { get; private set; }
    public ApplicationUser? Buyer { get; private set; }

    private WaitlistEntry() { } // EF Core

    public WaitlistEntry(
        Guid companyId,
        Guid branchId,
        Guid vehicleVariantId,
        Guid buyerId,
        int queuePosition,
        decimal tokenAmountPaid,
        string idempotencyKey)
    {
        CompanyId = companyId;
        BranchId = branchId;
        VehicleVariantId = vehicleVariantId;
        BuyerId = buyerId;
        QueuePosition = queuePosition;
        TokenAmountPaid = tokenAmountPaid;
        IdempotencyKey = idempotencyKey;
        Status = WaitlistStatus.Waiting;
    }

    /// <summary>
    /// Auto-allocates an incoming factory vehicle or cancelled stock to Token #1.
    /// </summary>
    public void AllocateStock(Guid stockId)
    {
        if (Status == WaitlistStatus.Waiting)
        {
            AllocatedStockId = stockId;
            AllocatedAt = DateTime.UtcNow;
            Status = WaitlistStatus.Allocated;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// 1-Click customer cancellation with 100% automated refund (terminal state).
    /// </summary>
    public void CancelAndRefund()
    {
        if (Status == WaitlistStatus.Waiting)
        {
            Status = WaitlistStatus.CancelledAndRefunded;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
