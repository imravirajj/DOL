using DOL.Identity.Domain.Enums;

namespace DOL.Identity.Application.DTOs;

// 1. Trade-In / Exchange DTOs
public record VehicleTradeInDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    string Make,
    string Model,
    int Year,
    int KilometersDriven,
    string FuelType,
    string Condition,
    bool HasAccidentHistory,
    string? RegistrationNumber,
    decimal EstimatedValue,
    decimal? OfferedValue,
    DateTime? InspectionDate,
    string? InspectorNotes,
    TradeInStatus Status,
    DateTime CreatedAt);

public record ValuateTradeInRequest(
    Guid CompanyId,
    Guid BranchId,
    string Make,
    string Model,
    int Year,
    int KilometersDriven,
    string FuelType,
    string Condition,
    bool HasAccidentHistory,
    string? RegistrationNumber = null);

public record BookTradeInInspectionRequest(
    DateTime InspectionDate);

public record ProvideTradeInOfferRequest(
    decimal OfferedValue,
    string? InspectorNotes = null);

// 2. Accessories DTOs
public record VehicleAccessoryDto(
    Guid Id,
    Guid CompanyId,
    string Name,
    string PartNumber,
    AccessoryCategory Category,
    Guid? CompatibleVariantId,
    decimal Price,
    decimal InstallationCost,
    int WarrantyMonths,
    bool IsActive);

public record CreateAccessoryRequest(
    Guid CompanyId,
    string Name,
    string PartNumber,
    AccessoryCategory Category,
    decimal Price,
    decimal InstallationCost = 0,
    int WarrantyMonths = 12,
    Guid? CompatibleVariantId = null);

public record UpdateAccessoryRequest(
    string Name,
    AccessoryCategory Category,
    decimal Price,
    decimal InstallationCost,
    int WarrantyMonths,
    Guid? CompatibleVariantId,
    bool IsActive);

// 3. Insurance DTOs
public record InsurancePolicyDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid OrderId,
    Guid BuyerId,
    string InsurerName,
    string PolicyNumber,
    string PlanType,
    decimal PremiumAmount,
    decimal IdvAmount,
    DateTime CoverageStartDate,
    DateTime CoverageEndDate,
    string? PolicyDocumentUrl,
    InsurancePolicyStatus Status,
    DateTime CreatedAt);

public record IssueInsurancePolicyRequest(
    Guid CompanyId,
    Guid BranchId,
    Guid OrderId,
    Guid BuyerId,
    string InsurerName,
    string PlanType,
    decimal PremiumAmount,
    decimal IdvAmount,
    DateTime CoverageStartDate,
    DateTime CoverageEndDate,
    string PolicyNumber,
    string? PolicyDocumentUrl = null);

public record InsurancePlanDto(
    string InsurerName,
    string PlanName,
    decimal AnnualPremium,
    decimal CashlessGaragesCount,
    bool ZeroDepIncluded,
    bool EngineProtectionIncluded,
    bool RoadsideAssistanceIncluded);

// 4. Service Appointment DTOs
public record ServiceAppointmentDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    string VinNumber,
    string RegistrationNumber,
    Guid? VehicleVariantId,
    ServiceType ServiceType,
    DateTime AppointmentDate,
    string TimeSlot,
    string? CustomerComments,
    decimal EstimatedCost,
    decimal? ActualCost,
    string? WorkshopNotes,
    ServiceAppointmentStatus Status,
    DateTime? CompletedAt,
    DateTime CreatedAt);

public record BookServiceAppointmentRequest(
    Guid CompanyId,
    Guid BranchId,
    string VinNumber,
    string RegistrationNumber,
    ServiceType ServiceType,
    DateTime AppointmentDate,
    string TimeSlot,
    decimal EstimatedCost = 1500,
    string? CustomerComments = null,
    Guid? VehicleVariantId = null);

public record UpdateServiceAppointmentRequest(
    decimal ActualCost,
    string? WorkshopNotes = null,
    ServiceAppointmentStatus Status = ServiceAppointmentStatus.Completed);

// 5. Notification DTOs
public record CustomerNotificationDto(
    Guid Id,
    Guid CompanyId,
    Guid UserId,
    string Title,
    string Message,
    NotificationChannel Channel,
    bool IsRead,
    DateTime? ReadAt,
    string? ActionUrl,
    DateTime CreatedAt);

public record SendNotificationRequest(
    Guid CompanyId,
    Guid UserId,
    string Title,
    string Message,
    NotificationChannel Channel = NotificationChannel.InApp,
    string? ActionUrl = null);

// 6. Review DTOs
public record DealershipReviewDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    Guid? OrderId,
    int Rating,
    string Title,
    string ReviewText,
    bool IsVerifiedBuyer,
    string? DealerResponse,
    DateTime? RespondedAt,
    DateTime CreatedAt);

public record CreateReviewRequest(
    Guid CompanyId,
    Guid BranchId,
    int Rating,
    string Title,
    string ReviewText,
    Guid? OrderId = null);

public record RespondToReviewRequest(
    string DealerResponse);

// 7. Analytics & BI DTOs
public record SalesFunnelDto(
    int TotalQuotations,
    int TotalOrders,
    int PendingLoans,
    int ApprovedLoans,
    int CompletedDeliveries,
    decimal LeadToOrderConversionPct,
    decimal OrderToDeliveryConversionPct);

public record StockAgingDto(
    int TotalVehiclesInStock,
    int Under30Days,
    int Between31And60Days,
    int Between61And90Days,
    int Over90Days,
    decimal TotalYardInventoryValue);

public record RevenueAnalyticsDto(
    decimal TotalOrderValue,
    decimal TotalBookingAmountCollected,
    decimal TotalDownPaymentCollected,
    decimal TotalLoanDisbursed,
    decimal TotalAccessoriesRevenue,
    decimal TotalServiceRevenue);
