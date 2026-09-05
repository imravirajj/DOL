namespace DOL.SharedKernel;

/// <summary>
/// Provides information about the currently authenticated user, company tenant, and branch scope.
/// </summary>
public interface ICurrentUserContext
{
    Guid? UserId { get; }
    string? Email { get; }
    Guid? CompanyId { get; }
    Guid? BranchId { get; }
    string? AccessScope { get; } // "CompanyLevel", "CountryLevel", "StateLevel", "CityLevel", "BranchLevel"
    Guid? ScopeEntityId { get; }
    IReadOnlyList<string> Roles { get; }

    bool IsAuthenticated { get; }
    bool IsCompanyAdmin { get; }
    bool IsGlobalAdmin { get; }
}
