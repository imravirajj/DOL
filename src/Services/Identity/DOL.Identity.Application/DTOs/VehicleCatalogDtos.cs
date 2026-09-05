namespace DOL.Identity.Application.DTOs;

public record VehicleModelDto(
    Guid Id,
    Guid CompanyId,
    string Make,
    string Model,
    int Year,
    string Category,
    bool IsActive,
    int VariantCount,
    DateTime CreatedAt);

public record VehicleVariantDto(
    Guid Id,
    Guid CompanyId,
    Guid VehicleModelId,
    string ModelName,
    string VariantName,
    string FuelType,
    string Transmission,
    decimal ExShowroomPrice,
    string ColorsAvailable,
    bool IsActive,
    int StockCount,
    DateTime CreatedAt);
