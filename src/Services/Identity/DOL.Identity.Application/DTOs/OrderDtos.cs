using DOL.Identity.Domain.Enums;

namespace DOL.Identity.Application.DTOs;

public record VehicleOrderDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    Guid QuotationId,
    Guid VehicleVariantId,
    Guid? AllocatedStockId,
    string? AllocatedVin,
    string OrderNumber,
    decimal TotalAmount,
    decimal BookingAmountPaid,
    decimal DownPaymentPaid,
    decimal LoanDisbursedAmount,
    OrderStatus Status,
    DeliveryType DeliveryType,
    string DeliveryOtp,
    DateTime? DeliveredAt,
    DateTime CreatedAt);
