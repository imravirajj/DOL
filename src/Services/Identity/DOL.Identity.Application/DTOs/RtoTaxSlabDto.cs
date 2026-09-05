namespace DOL.Identity.Application.DTOs;

public record RtoTaxSlabDto(
    Guid Id,
    Guid CompanyId,
    string StateName,
    string FuelType,
    decimal TaxPercentage,
    decimal CessPercentage);
