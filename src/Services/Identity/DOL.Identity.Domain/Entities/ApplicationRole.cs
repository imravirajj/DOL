using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class ApplicationRole : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    // Static GUIDs for deterministic seeding
    public static readonly Guid AdminRoleId = Guid.Parse("c0a80101-0000-0000-0000-000000000001");
    public static readonly Guid BuyerRoleId = Guid.Parse("c0a80101-0000-0000-0000-000000000002");
    public static readonly Guid DealerRoleId = Guid.Parse("c0a80101-0000-0000-0000-000000000003");
    public static readonly Guid CompanyAdminRoleId = Guid.Parse("c0a80101-0000-0000-0000-000000000004");
    public static readonly Guid BranchManagerRoleId = Guid.Parse("c0a80101-0000-0000-0000-000000000005");
    public static readonly Guid BranchStaffRoleId = Guid.Parse("c0a80101-0000-0000-0000-000000000006");

    // EF Core constructor
    private ApplicationRole() { }

    public ApplicationRole(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public ApplicationRole(Guid id, string name, string description, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
    }

    // Well-known Role Names
    public const string Admin = "Admin";
    public const string Buyer = "Buyer";
    public const string Dealer = "Dealer";
    public const string CompanyAdmin = "CompanyAdmin";
    public const string BranchManager = "BranchManager";
    public const string BranchStaff = "BranchStaff";
}
