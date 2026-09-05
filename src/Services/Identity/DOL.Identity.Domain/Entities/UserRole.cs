namespace DOL.Identity.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; } = null!;

    public Guid RoleId { get; private set; }
    public ApplicationRole Role { get; private set; } = null!;

    private UserRole() { }

    public UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
