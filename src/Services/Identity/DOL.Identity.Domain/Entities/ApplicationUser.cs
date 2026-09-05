using DOL.Identity.Domain.Enums;
using DOL.Identity.Domain.Events;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class ApplicationUser : AuditableEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserStatus Status { get; private set; } = UserStatus.Active;
    public bool EmailConfirmed { get; private set; } = false;
    public int AccessFailedCount { get; private set; } = 0;
    public DateTime? LockoutEnd { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiresAt { get; private set; }

    public Guid? CompanyId { get; private set; }
    public AccessScope Scope { get; private set; } = AccessScope.BranchLevel;
    public Guid? ScopeEntityId { get; private set; }
    public Guid? BranchId { get; private set; }

    public Company? Company { get; private set; }
    public Branch? Branch { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    // EF Core constructor
    private ApplicationUser() { }

    public ApplicationUser(string firstName, string lastName, string email, string phoneNumber, string passwordHash)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email.ToLowerInvariant().Trim();
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
        Status = UserStatus.Active;

        AddDomainEvent(new UserRegisteredEvent(Id, Email));
    }

    public ApplicationUser(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string passwordHash,
        Guid companyId,
        AccessScope scope,
        Guid? scopeEntityId = null,
        Guid? branchId = null)
        : this(firstName, lastName, email, phoneNumber, passwordHash)
    {
        CompanyId = companyId;
        Scope = scope;
        ScopeEntityId = scopeEntityId;
        BranchId = branchId;
    }

    public void AssignCompanyAndScope(Guid companyId, AccessScope scope, Guid? scopeEntityId = null, Guid? branchId = null)
    {
        CompanyId = companyId;
        Scope = scope;
        ScopeEntityId = scopeEntityId;
        BranchId = branchId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRole(Guid roleId)
    {
        if (!_userRoles.Any(ur => ur.RoleId == roleId))
        {
            _userRoles.Add(new UserRole(Id, roleId));
        }
    }

    public void RemoveRole(Guid roleId)
    {
        var role = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
        if (role != null)
        {
            _userRoles.Remove(role);
        }
    }

    public void AddRefreshToken(string token, DateTime expiresAt, string createdByIp)
    {
        _refreshTokens.Add(new RefreshToken(Id, token, expiresAt, createdByIp));
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordFailedLogin()
    {
        AccessFailedCount++;
        if (AccessFailedCount >= 5)
        {
            LockoutEnd = DateTime.UtcNow.AddMinutes(15);
        }
    }

    public void ResetFailedLogin()
    {
        AccessFailedCount = 0;
        LockoutEnd = null;
    }

    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;

    public void SetPasswordResetToken(string token, TimeSpan validity)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiresAt = DateTime.UtcNow.Add(validity);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool ValidatePasswordResetToken(string token)
    {
        if (string.IsNullOrEmpty(PasswordResetToken) || PasswordResetToken != token)
            return false;

        if (!PasswordResetTokenExpiresAt.HasValue || PasswordResetTokenExpiresAt.Value < DateTime.UtcNow)
            return false;

        return true;
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiresAt = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
