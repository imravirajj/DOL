namespace DOL.Identity.Application.DTOs;

public record CompanyDto(
    Guid Id,
    string Name,
    string Code,
    string Email,
    string PhoneNumber,
    string? Address,
    string SubscriptionPlan,
    string Status,
    string Currency,
    string TimeZone,
    DateTime CreatedAt,
    int TotalBranches = 0
);

public record CountryDto(
    Guid Id,
    Guid CompanyId,
    string Name,
    string IsoCode
);

public record StateRegionDto(
    Guid Id,
    Guid CompanyId,
    Guid CountryId,
    string Name,
    string? StateCode
);

public record CityDto(
    Guid Id,
    Guid CompanyId,
    Guid StateRegionId,
    string Name
);

public record BranchDto(
    Guid Id,
    Guid CompanyId,
    Guid CityId,
    string Name,
    string BranchCode,
    string Address,
    string? ContactPhone,
    string? ContactEmail,
    bool IsActive,
    bool IsMainBranch,
    DateTime CreatedAt,
    string? CityName = null,
    string? StateName = null,
    string? CountryName = null
);
