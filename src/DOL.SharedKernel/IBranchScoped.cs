namespace DOL.SharedKernel;

/// <summary>
/// Marks an entity as belonging to a specific branch within a company.
/// Used for branch-level strict data isolation.
/// </summary>
public interface IBranchScoped : ITenantScoped
{
    Guid BranchId { get; }
}
