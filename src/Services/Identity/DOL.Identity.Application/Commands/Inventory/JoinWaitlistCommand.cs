using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Inventory;

public record JoinWaitlistCommand(
    Guid VehicleVariantId,
    Guid BranchId,
    string IdempotencyKey,
    decimal TokenAmountPaid = 11000
) : IRequest<Result<WaitlistResultDto>>;

public class JoinWaitlistCommandHandler : IRequestHandler<JoinWaitlistCommand, Result<WaitlistResultDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;

    public JoinWaitlistCommandHandler(
        IIdentityDbContext context,
        ICurrentUserContext currentUserContext)
    {
        _context = context;
        _currentUserContext = currentUserContext;
    }

    public async Task<Result<WaitlistResultDto>> Handle(JoinWaitlistCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserContext.UserId.HasValue)
        {
            return Result.Failure<WaitlistResultDto>("Authentication required to join the priority waitlist.");
        }

        var buyerId = _currentUserContext.UserId.Value;

        // Verify variant and branch exist
        var variant = await _context.VehicleVariants
            .Include(v => v.VehicleModel)
            .FirstOrDefaultAsync(v => v.Id == request.VehicleVariantId, cancellationToken);

        if (variant == null)
        {
            return Result.Failure<WaitlistResultDto>("Vehicle variant not found.");
        }

        var branch = await _context.Branches
            .FirstOrDefaultAsync(b => b.Id == request.BranchId, cancellationToken);

        if (branch == null)
        {
            return Result.Failure<WaitlistResultDto>("Target branch not found.");
        }

        // Idempotency: Check if an entry with this idempotency key already exists
        var existingByKey = await _context.WaitlistEntries
            .FirstOrDefaultAsync(w => w.IdempotencyKey == request.IdempotencyKey, cancellationToken);

        if (existingByKey != null)
        {
            return Result.Success(new WaitlistResultDto(
                existingByKey.Id,
                existingByKey.VehicleVariantId,
                existingByKey.QueuePosition,
                existingByKey.TokenAmountPaid,
                existingByKey.Status.ToString(),
                $"You are already confirmed at Queue Token #{existingByKey.QueuePosition}.",
                "3-4 weeks"
            ));
        }

        // Calculate next FIFO Queue Position
        var currentMaxQueue = await _context.WaitlistEntries
            .Where(w => w.BranchId == request.BranchId &&
                        w.VehicleVariantId == request.VehicleVariantId &&
                        w.Status == WaitlistStatus.Waiting)
            .MaxAsync(w => (int?)w.QueuePosition, cancellationToken) ?? 0;

        var nextPosition = currentMaxQueue + 1;

        var waitlistEntry = new WaitlistEntry(
            variant.CompanyId,
            request.BranchId,
            request.VehicleVariantId,
            buyerId,
            nextPosition,
            request.TokenAmountPaid,
            request.IdempotencyKey
        );

        _context.WaitlistEntries.Add(waitlistEntry);
        await _context.SaveChangesAsync(cancellationToken);

        var estimatedWeeks = nextPosition <= 2 ? "2-3 weeks" : $"{nextPosition * 10}-{(nextPosition * 10) + 7} days";

        return Result.Success(new WaitlistResultDto(
            waitlistEntry.Id,
            waitlistEntry.VehicleVariantId,
            nextPosition,
            waitlistEntry.TokenAmountPaid,
            waitlistEntry.Status.ToString(),
            $"Successfully enrolled in Priority Factory Allocation! You are Queue Token #{nextPosition}.",
            estimatedWeeks
        ));
    }
}
