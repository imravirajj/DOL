using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class VehicleStock : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid VehicleVariantId { get; private set; }

    public string VinNumber { get; private set; } = string.Empty;    // Unique 17-char VIN
    public string EngineNumber { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;
    public VehicleStockStatus Status { get; private set; } = VehicleStockStatus.Available;

    // Concurrency Lock & Reservation
    public Guid? ReservedByBuyerId { get; private set; }
    public DateTime? ReservationExpiresAt { get; private set; }
    public Guid? ConfirmedOrderId { get; private set; }
    public uint Version { get; private set; } = 1; // Optimistic Concurrency Token

    public Branch? Branch { get; private set; }
    public VehicleVariant? VehicleVariant { get; private set; }

    private VehicleStock() { } // EF Core

    public VehicleStock(
        Guid companyId,
        Guid branchId,
        Guid vehicleVariantId,
        string vinNumber,
        string engineNumber,
        string color)
    {
        CompanyId = companyId;
        BranchId = branchId;
        VehicleVariantId = vehicleVariantId;
        VinNumber = vinNumber.Trim().ToUpperInvariant();
        EngineNumber = engineNumber.Trim().ToUpperInvariant();
        Color = color.Trim();
        Status = VehicleStockStatus.Available;
    }

    /// <summary>
    /// Attempts an atomic reservation hold (e.g. 15 minutes).
    /// Solves race conditions when multiple buyers try to book the same vehicle.
    /// </summary>
    public bool TryReserve(Guid buyerId, TimeSpan holdDuration)
    {
        var now = DateTime.UtcNow;

        // Vehicle can only be reserved if it's Available or previously expired
        bool isEligible = Status == VehicleStockStatus.Available ||
                          (Status == VehicleStockStatus.Reserved && ReservationExpiresAt.HasValue && ReservationExpiresAt.Value < now);

        if (!isEligible)
        {
            return false;
        }

        Status = VehicleStockStatus.Reserved;
        ReservedByBuyerId = buyerId;
        ReservationExpiresAt = now.Add(holdDuration);
        Version++;
        UpdatedAt = now;
        return true;
    }

    /// <summary>
    /// Confirms permanent booking once payment succeeds within the 15-min TTL hold.
    /// </summary>
    public bool ConfirmBooking(Guid buyerId, Guid orderId)
    {
        var now = DateTime.UtcNow;

        if (Status != VehicleStockStatus.Reserved ||
            ReservedByBuyerId != buyerId ||
            (ReservationExpiresAt.HasValue && ReservationExpiresAt.Value < now))
        {
            return false;
        }

        Status = VehicleStockStatus.Booked;
        ConfirmedOrderId = orderId;
        ReservationExpiresAt = null;
        Version++;
        UpdatedAt = now;
        return true;
    }

    /// <summary>
    /// Releases the temporary hold if the buyer abandons payment or 15 minutes expire.
    /// </summary>
    public void ReleaseReservation()
    {
        if (Status == VehicleStockStatus.Reserved)
        {
            Status = VehicleStockStatus.Available;
            ReservedByBuyerId = null;
            ReservationExpiresAt = null;
            Version++;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Supports Inter-Branch Stock Transfer if nearby branch needs stock.
    /// </summary>
    public void TransferToBranch(Guid targetBranchId)
    {
        if (Status == VehicleStockStatus.Available)
        {
            BranchId = targetBranchId;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateDetails(string color, string engineNumber)
    {
        Color = color.Trim();
        EngineNumber = engineNumber.Trim().ToUpperInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(VehicleStockStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
