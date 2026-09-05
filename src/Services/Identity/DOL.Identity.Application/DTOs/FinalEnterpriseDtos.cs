using DOL.Identity.Domain.Enums;

namespace DOL.Identity.Application.DTOs;

// 1. Payment DTOs
public record PaymentTransactionDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    Guid? OrderId,
    Guid? QuotationId,
    string TransactionReference,
    string GatewayProvider,
    string? GatewayPaymentId,
    string? GatewayOrderId,
    decimal Amount,
    string Currency,
    PaymentPurpose Purpose,
    PaymentStatus Status,
    string PaymentMode,
    DateTime? PaidAt,
    string? FailureReason,
    string? ReceiptUrl,
    DateTime CreatedAt);

public record InitiatePaymentRequest(
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    decimal Amount,
    PaymentPurpose Purpose,
    string GatewayProvider = "Razorpay",
    string PaymentMode = "UPI",
    Guid? OrderId = null,
    Guid? QuotationId = null);

public record VerifyPaymentRequest(
    string TransactionReference,
    string GatewayPaymentId,
    string? Signature = null,
    string? ReceiptUrl = null);

public record RefundPaymentRequest(
    string? Reason = null);

// 2. KYC Document DTOs
public record CustomerDocumentDto(
    Guid Id,
    Guid CompanyId,
    Guid UserId,
    Guid? OrderId,
    DocumentType DocumentType,
    string DocumentNumber,
    string FileUrl,
    string FileName,
    long FileSizeBytes,
    DocumentVerificationStatus VerificationStatus,
    Guid? VerifiedByStaffId,
    DateTime? VerifiedAt,
    string? RejectionReason,
    DateTime CreatedAt);

public record UploadDocumentRequest(
    Guid CompanyId,
    Guid UserId,
    DocumentType DocumentType,
    string DocumentNumber,
    string FileUrl,
    string FileName,
    long FileSizeBytes,
    Guid? OrderId = null);

public record VerifyDocumentRequest(
    Guid StaffId,
    bool Approve,
    string? RejectionReason = null);

// 3. Warranty & AMC DTOs
public record WarrantyPackageDto(
    Guid Id,
    Guid CompanyId,
    string Name,
    WarrantyPackageType PackageType,
    int DurationMonths,
    int KilometerLimit,
    decimal Price,
    string Description,
    bool IsActive);

public record CreateWarrantyPackageRequest(
    Guid CompanyId,
    string Name,
    WarrantyPackageType PackageType,
    int DurationMonths,
    int KilometerLimit,
    decimal Price,
    string Description);

public record VehicleWarrantySubscriptionDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    Guid? OrderId,
    Guid WarrantyPackageId,
    string VinNumber,
    string SubscriptionNumber,
    DateTime StartDate,
    DateTime EndDate,
    decimal PricePaid,
    WarrantyStatus Status,
    DateTime CreatedAt);

public record SubscribeWarrantyRequest(
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    Guid WarrantyPackageId,
    string VinNumber,
    Guid? OrderId = null);

// 4. Sales CRM DTOs
public record SalesLeadDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid? AssignedStaffId,
    Guid? InterestedModelId,
    string CustomerName,
    string CustomerPhone,
    string? CustomerEmail,
    string LeadSource,
    LeadPriority Priority,
    LeadStage Stage,
    string? Notes,
    DateTime? NextFollowUpDate,
    string? LostReason,
    DateTime CreatedAt);

public record CreateLeadRequest(
    Guid CompanyId,
    Guid BranchId,
    string CustomerName,
    string CustomerPhone,
    string? CustomerEmail = null,
    string LeadSource = "Website",
    LeadPriority Priority = LeadPriority.Hot,
    Guid? InterestedModelId = null,
    Guid? AssignedStaffId = null,
    string? Notes = null,
    DateTime? NextFollowUpDate = null);

public record UpdateLeadStageRequest(
    LeadStage Stage,
    string? LostReason = null);

public record AssignLeadRequest(
    Guid StaffId);

public record ScheduleFollowUpRequest(
    DateTime NextFollowUpDate,
    string? Notes = null);

// 5. EV DTOs
public record EvChargingStationDto(
    Guid Id,
    Guid CompanyId,
    Guid? BranchId,
    string StationName,
    string LocationAddress,
    double Latitude,
    double Longitude,
    string ConnectorType,
    int PowerKw,
    decimal TariffPerKwh,
    bool IsAvailable);

public record CreateChargingStationRequest(
    Guid CompanyId,
    string StationName,
    string LocationAddress,
    double Latitude,
    double Longitude,
    string ConnectorType = "CCS2",
    int PowerKw = 60,
    decimal TariffPerKwh = 18.5m,
    Guid? BranchId = null);

public record HomeChargerInstallationDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    Guid? OrderId,
    string InstallationAddress,
    DateTime PreferredSurveyDate,
    string ChargerModel,
    HomeChargerSurveyStatus SurveyStatus,
    string? TechnicianNotes,
    DateTime? InstalledAt,
    DateTime CreatedAt);

public record RequestHomeChargerRequest(
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    string InstallationAddress,
    DateTime PreferredSurveyDate,
    string ChargerModel = "7.4 kW AC Fast Home Charger",
    Guid? OrderId = null);

public record UpdateHomeChargerSurveyRequest(
    HomeChargerSurveyStatus Status,
    string? TechnicianNotes = null);
