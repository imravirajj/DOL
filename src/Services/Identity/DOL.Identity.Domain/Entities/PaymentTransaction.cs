using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class PaymentTransaction : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid? OrderId { get; private set; }
    public Guid? QuotationId { get; private set; }

    public string TransactionReference { get; private set; } = string.Empty; // e.g. "TXN-2026-MUM-88210"
    public string GatewayProvider { get; private set; } = "Razorpay";        // Razorpay, Stripe, CashFree, UPI
    public string? GatewayPaymentId { get; private set; }
    public string? GatewayOrderId { get; private set; }

    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "INR";
    public PaymentPurpose Purpose { get; private set; } = PaymentPurpose.BookingToken;
    public PaymentStatus Status { get; private set; } = PaymentStatus.Initiated;
    public string PaymentMode { get; private set; } = "UPI"; // UPI, Card, NetBanking, Cash
    public DateTime? PaidAt { get; private set; }
    public string? FailureReason { get; private set; }
    public string? ReceiptUrl { get; private set; }

    public VehicleOrder? Order { get; private set; }
    public ApplicationUser? Buyer { get; private set; }

    private PaymentTransaction() { } // EF Core

    public PaymentTransaction(
        Guid companyId,
        Guid branchId,
        Guid buyerId,
        string transactionReference,
        decimal amount,
        PaymentPurpose purpose,
        string gatewayProvider = "Razorpay",
        string paymentMode = "UPI",
        Guid? orderId = null,
        Guid? quotationId = null,
        string? gatewayOrderId = null)
    {
        CompanyId = companyId;
        BranchId = branchId;
        BuyerId = buyerId;
        TransactionReference = transactionReference.Trim();
        Amount = amount;
        Purpose = purpose;
        GatewayProvider = gatewayProvider.Trim();
        PaymentMode = paymentMode.Trim();
        OrderId = orderId;
        QuotationId = quotationId;
        GatewayOrderId = gatewayOrderId?.Trim();
        Status = PaymentStatus.Initiated;
    }

    public void MarkSuccessful(string gatewayPaymentId, string? receiptUrl = null)
    {
        GatewayPaymentId = gatewayPaymentId.Trim();
        ReceiptUrl = receiptUrl?.Trim();
        Status = PaymentStatus.Successful;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string reason)
    {
        FailureReason = reason.Trim();
        Status = PaymentStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ProcessRefund(string? reason = null)
    {
        Status = PaymentStatus.Refunded;
        FailureReason = reason?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
