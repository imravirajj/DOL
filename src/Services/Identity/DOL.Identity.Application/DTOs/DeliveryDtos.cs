namespace DOL.Identity.Application.DTOs;

public record DeliveryInspectionDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid OrderId,
    Guid VehicleStockId,
    Guid InspectorStaffId,
    int OdometerReadingKm,
    int BatteryHealthPct,
    bool ExteriorConditionOk,
    bool InteriorCleanOk,
    bool ToolKitAndSpareWheelOk,
    bool DocumentationOk,
    string? InspectionNotes,
    bool IsCustomerAccepted,
    string? CustomerSignatureUrl,
    DateTime CreatedAt);
