using DOL.Identity.Domain.Enums;

namespace DOL.Identity.Application.DTOs;

public record TestDriveBookingDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    Guid VehicleVariantId,
    string? VariantName,
    string CustomerName,
    string CustomerPhone,
    string CustomerEmail,
    string DrivingLicenseNumber,
    DateTime ScheduledDate,
    string TimeSlot,
    DeliveryType LocationType,
    string? HomeAddress,
    TestDriveStatus Status,
    int? Rating,
    string? FeedbackNotes,
    DateTime CreatedAt);
