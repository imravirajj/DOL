namespace DOL.Identity.Domain.Enums;

/// <summary>
/// Defines the level of visibility and control a user has within their company.
/// </summary>
public enum AccessScope
{
    /// <summary>
    /// Company HQ Super Admin — Full visibility across all countries, states, cities, and branches.
    /// </summary>
    CompanyLevel = 1,

    /// <summary>
    /// Regional / Country Director — Access to all branches within the designated country.
    /// </summary>
    CountryLevel = 2,

    /// <summary>
    /// State / Province Manager — Access to all branches within the designated state/region.
    /// </summary>
    StateLevel = 3,

    /// <summary>
    /// City / Metro Supervisor — Access to all branches within the designated city.
    /// </summary>
    CityLevel = 4,

    /// <summary>
    /// Branch Manager / Staff — Strictly confined to their assigned branch.
    /// </summary>
    BranchLevel = 5
}
