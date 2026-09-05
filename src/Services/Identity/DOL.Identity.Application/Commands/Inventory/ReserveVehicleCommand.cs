using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Inventory;

public record ReserveVehicleCommand(
    Guid VehicleStockId,
    int HoldMinutes = 15
) : IRequest<Result<ReservationResultDto>>;

public class ReserveVehicleCommandHandler : IRequestHandler<ReserveVehicleCommand, Result<ReservationResultDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;

    public ReserveVehicleCommandHandler(
        IIdentityDbContext context,
        ICurrentUserContext currentUserContext)
    {
        _context = context;
        _currentUserContext = currentUserContext;
    }

    public async Task<Result<ReservationResultDto>> Handle(ReserveVehicleCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserContext.UserId.HasValue)
        {
            return Result.Failure<ReservationResultDto>("User authentication required to reserve a vehicle.");
        }

        var buyerId = _currentUserContext.UserId.Value;
        var holdDuration = TimeSpan.FromMinutes(request.HoldMinutes > 0 && request.HoldMinutes <= 30 ? request.HoldMinutes : 15);

        // Fetch vehicle with tracking to perform atomic check-and-set
        var vehicle = await _context.VehicleStocks
            .FirstOrDefaultAsync(s => s.Id == request.VehicleStockId, cancellationToken);

        if (vehicle == null)
        {
            return Result.Failure<ReservationResultDto>("Vehicle was not found in inventory.");
        }

        // Concurrency Check: If already reserved by this user and not expired, refresh hold
        if (vehicle.Status == VehicleStockStatus.Reserved && vehicle.ReservedByBuyerId == buyerId &&
            vehicle.ReservationExpiresAt.HasValue && vehicle.ReservationExpiresAt.Value > DateTime.UtcNow)
        {
            var remaining = (int)(vehicle.ReservationExpiresAt.Value - DateTime.UtcNow).TotalSeconds;
            return Result.Success(new ReservationResultDto(
                true,
                vehicle.Id,
                vehicle.VinNumber,
                vehicle.ReservationExpiresAt.Value,
                "You already have an active reservation hold on this vehicle.",
                remaining
            ));
        }

        // Atomic Reservation Lock
        var reserved = vehicle.TryReserve(buyerId, holdDuration);
        if (!reserved)
        {
            return Result.Failure<ReservationResultDto>(
                "This vehicle is currently locked by another customer completing checkout. " +
                "You can join the priority waitlist or choose another variant/color."
            );
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another transaction won the race at the exact same millisecond!
            return Result.Failure<ReservationResultDto>(
                "This vehicle was just reserved by another customer. Please choose an alternate vehicle or join the waitlist."
            );
        }

        var expiresAt = vehicle.ReservationExpiresAt ?? DateTime.UtcNow.Add(holdDuration);
        var remainingSeconds = (int)(expiresAt - DateTime.UtcNow).TotalSeconds;

        return Result.Success(new ReservationResultDto(
            true,
            vehicle.Id,
            vehicle.VinNumber,
            expiresAt,
            $"Vehicle locked exclusively for you for {request.HoldMinutes} minutes. Please complete booking payment.",
            remainingSeconds
        ));
    }
}
