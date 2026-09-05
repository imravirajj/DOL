namespace DOL.Identity.Application.DTOs;

public record VehicleStockDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid VehicleVariantId,
    string VinNumber,
    string EngineNumber,
    string Color,
    string Status,
    string? BranchName = null,
    string? VariantName = null,
    string? Make = null,
    string? Model = null,
    decimal ExShowroomPrice = 0,
    bool IsHeldByCurrentUser = false,
    DateTime? ReservationExpiresAt = null
);

public record ReservationResultDto(
    bool IsSuccess,
    Guid VehicleStockId,
    string VinNumber,
    DateTime ExpiresAt,
    string Message,
    int RemainingSeconds
);

public record WaitlistResultDto(
    Guid WaitlistId,
    Guid VehicleVariantId,
    int QueuePosition,
    decimal TokenAmountPaid,
    string Status,
    string Message,
    string EstimatedWaitTime
);

public record InterBranchStockDto(
    Guid StockId,
    Guid BranchId,
    string BranchName,
    string CityName,
    string VinNumber,
    string Color,
    decimal ExShowroomPrice,
    string TransferEstimatedDays
);
