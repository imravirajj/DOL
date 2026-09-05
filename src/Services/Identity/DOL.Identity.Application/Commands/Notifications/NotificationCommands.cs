using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Notifications;

// ── Send Notification ───────────────────────────────────────
public record SendNotificationCommand(
    Guid CompanyId,
    Guid UserId,
    string Title,
    string Message,
    NotificationChannel Channel = NotificationChannel.InApp,
    string? ActionUrl = null) : IRequest<Result<Guid>>;

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Message).NotEmpty();
    }
}

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public SendNotificationCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = new CustomerNotification(
            request.CompanyId,
            request.UserId,
            request.Title,
            request.Message,
            request.Channel,
            request.ActionUrl);

        _context.CustomerNotifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(notification.Id);
    }
}

// ── Mark Notification As Read ───────────────────────────────
public record MarkNotificationAsReadCommand(Guid Id) : IRequest<Result<bool>>;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public MarkNotificationAsReadCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var n = await _context.CustomerNotifications.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (n == null) return Result<bool>.Failure("Notification not found.");

        n.MarkAsRead();
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Get User Notifications ──────────────────────────────────
public record GetMyNotificationsQuery(Guid UserId, bool? UnreadOnly = null) : IRequest<Result<List<CustomerNotificationDto>>>;

public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Result<List<CustomerNotificationDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetMyNotificationsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CustomerNotificationDto>>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.CustomerNotifications.AsNoTracking().Where(n => n.UserId == request.UserId);

        if (request.UnreadOnly == true)
        {
            query = query.Where(n => !n.IsRead);
        }

        var list = await query
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new CustomerNotificationDto(
                n.Id,
                n.CompanyId,
                n.UserId,
                n.Title,
                n.Message,
                n.Channel,
                n.IsRead,
                n.ReadAt,
                n.ActionUrl,
                n.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<CustomerNotificationDto>>.Success(list);
    }
}
