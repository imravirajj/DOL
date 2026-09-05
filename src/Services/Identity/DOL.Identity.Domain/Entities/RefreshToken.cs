using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public string CreatedByIp { get; private set; } = string.Empty;
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    // EF Core constructor
    private RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expiresAt, string createdByIp)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedByIp = createdByIp;
    }

    public void Revoke(string? replacedByToken = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
    }
}
