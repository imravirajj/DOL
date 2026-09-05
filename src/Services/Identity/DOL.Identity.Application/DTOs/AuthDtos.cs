namespace DOL.Identity.Application.DTOs;

public record UserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Status,
    bool EmailConfirmed,
    List<string> Roles,
    DateTime CreatedAt,
    Guid? CompanyId = null,
    Guid? BranchId = null,
    string? AccessScope = null,
    Guid? ScopeEntityId = null
);

public record TokenResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);

public record AuthResultDto(
    UserDto User,
    TokenResponseDto Tokens
);
