using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class CustomerNotification : AuditableEntity, ITenantScoped
{
    public Guid CompanyId { get; private set; }
    public Guid UserId { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public NotificationChannel Channel { get; private set; } = NotificationChannel.InApp;
    public bool IsRead { get; private set; } = false;
    public DateTime? ReadAt { get; private set; }
    public string? ActionUrl { get; private set; }

    public ApplicationUser? User { get; private set; }

    private CustomerNotification() { } // EF Core

    public CustomerNotification(
        Guid companyId,
        Guid userId,
        string title,
        string message,
        NotificationChannel channel = NotificationChannel.InApp,
        string? actionUrl = null)
    {
        CompanyId = companyId;
        UserId = userId;
        Title = title.Trim();
        Message = message.Trim();
        Channel = channel;
        ActionUrl = actionUrl?.Trim();
        IsRead = false;
    }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
