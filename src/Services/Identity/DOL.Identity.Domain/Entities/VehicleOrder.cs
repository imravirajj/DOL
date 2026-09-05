using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class VehicleOrder : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid QuotationId { get; private set; }
    public Guid VehicleVariantId { get; private set; }
    public Guid? AllocatedStockId { get; private set; }

    public string OrderNumber { get; private set; } = string.Empty; // e.g. "ORD-2026-MUM-00101"
    public decimal TotalAmount { get; private set; }
    public decimal BookingAmountPaid { get; private set; }
    public decimal DownPaymentPaid { get; private set; }
    public decimal LoanDisbursedAmount { get; private set; }

    public OrderStatus Status { get; private set; } = OrderStatus.Booked;
    public DeliveryType DeliveryType { get; private set; } = DeliveryType.ShowroomPickup;
    public string DeliveryOtp { get; private set; } = string.Empty;
    public DateTime? DeliveredAt { get; private set; }

    public Quotation? Quotation { get; private set; }
    public VehicleVariant? VehicleVariant { get; private set; }
    public VehicleStock? AllocatedStock { get; private set; }

    private VehicleOrder() { } // EF Core

    public VehicleOrder(
        Guid companyId,
        Guid branchId,
        Guid buyerId,
        Guid quotationId,
        Guid vehicleVariantId,
        string orderNumber,
        decimal totalAmount,
        decimal bookingAmountPaid,
        DeliveryType deliveryType,
        Guid? allocatedStockId = null)
    {
        CompanyId = companyId;
        BranchId = branchId;
        BuyerId = buyerId;
        QuotationId = quotationId;
        VehicleVariantId = vehicleVariantId;
        OrderNumber = orderNumber.Trim().ToUpperInvariant();
        TotalAmount = totalAmount;
        BookingAmountPaid = bookingAmountPaid;
        DeliveryType = deliveryType;
        AllocatedStockId = allocatedStockId;
        Status = OrderStatus.Booked;
        DeliveryOtp = new Random().Next(100000, 999999).ToString();
    }

    public void AllocateStock(Guid stockId)
    {
        AllocatedStockId = stockId;
        Status = OrderStatus.VinAllocated;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordDownPayment(decimal amount)
    {
        DownPaymentPaid += amount;
        Status = OrderStatus.DownPaymentReceived;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordLoanDisbursal(decimal amount)
    {
        LoanDisbursedAmount += amount;
        Status = OrderStatus.LoanSanctioned;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AdvanceStatus(OrderStatus nextStatus)
    {
        Status = nextStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool VerifyAndDeliver(string enteredOtp)
    {
        if (Status != OrderStatus.PdiReady && Status != OrderStatus.RtoCompleted && Status != OrderStatus.VinAllocated)
        {
            return false;
        }

        if (DeliveryOtp != enteredOtp.Trim())
        {
            return false;
        }

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    public void RegenerateOtp()
    {
        DeliveryOtp = new Random().Next(100000, 999999).ToString();
        UpdatedAt = DateTime.UtcNow;
    }
}
