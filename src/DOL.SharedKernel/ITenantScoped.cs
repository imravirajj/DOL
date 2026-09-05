namespace DOL.SharedKernel;

/// <summary>
/// Marks an entity as belonging to a specific tenant / company.
/// Used for multi-tenant data isolation.
/// </summary>
public interface ITenantScoped
{
    Guid CompanyId { get; }
}
