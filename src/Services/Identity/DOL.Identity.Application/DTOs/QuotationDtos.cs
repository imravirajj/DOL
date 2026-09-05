namespace DOL.Identity.Application.DTOs;

public record QuotationDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid VehicleVariantId,
    Guid? BuyerId,
    string QuotationNumber,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string SelectedColor,
    string Make,
    string Model,
    string VariantName,
    string FuelType,
    string Transmission,
    string BranchName,
    string StateName,
    // Price Breakdown
    decimal ExShowroomPrice,
    decimal RtoTaxAmount,
    decimal InsuranceBaseAmount,
    decimal InsuranceAddonsAmount,
    decimal FastagCharges,
    decimal TcsAmount,
    decimal AccessoriesTotal,
    decimal ExtendedWarrantyAmount,
    decimal DiscountAmount,
    decimal TotalOnRoadPrice,
    // Addon Flags
    bool IncludeZeroDep,
    bool IncludeEngineProtect,
    bool IncludeReturnToInvoice,
    bool IncludeExtendedWarranty,
    string Status,
    DateTime ValidUntil,
    DateTime CreatedAt
);

public record QuotationSummaryDto(
    Guid Id,
    string QuotationNumber,
    string CustomerName,
    string Make,
    string Model,
    string VariantName,
    decimal TotalOnRoadPrice,
    string Status,
    DateTime ValidUntil,
    DateTime CreatedAt
);
