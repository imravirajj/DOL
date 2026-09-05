using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Inventory;

public record ConfirmBookingCommand(
    Guid VehicleStockId,
    string IdempotencyKey,
    decimal BookingAmountPaid = 25000
) : IRequest<Result<VehicleStockDto>>;

public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, Result<VehicleStockDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;

    public ConfirmBookingCommandHandler(
        IIdentityDbContext context,
        ICurrentUserContext currentUserContext)
    {
        _context = context;
        _currentUserContext = currentUserContext;
    }

    public async Task<Result<VehicleStockDto>> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserContext.UserId.HasValue)
        {
            return Result.Failure<VehicleStockDto>("User authentication required to confirm booking.");
        }

        var buyerId = _currentUserContext.UserId.Value;

        var vehicle = await _context.VehicleStocks
            .Include(s => s.Branch)
            .Include(s => s.VehicleVariant)
                .ThenInclude(v => v!.VehicleModel)
            .FirstOrDefaultAsync(s => s.Id == request.VehicleStockId, cancellationToken);

        if (vehicle == null)
        {
            return Result.Failure<VehicleStockDto>("Vehicle was not found in inventory.");
        }

        // Idempotency: If already booked by this user, return existing booking cleanly without error
        if (vehicle.Status == VehicleStockStatus.Booked && vehicle.ReservedByBuyerId == buyerId)
        {
            return Result.Success(MapToDto(vehicle, true));
        }

        // Validate active reservation
        if (vehicle.Status != VehicleStockStatus.Reserved || vehicle.ReservedByBuyerId != buyerId)
        {
            return Result.Failure<VehicleStockDto>("No active reservation hold found for this vehicle. Please reserve it first.");
        }

        if (vehicle.ReservationExpiresAt.HasValue && vehicle.ReservationExpiresAt.Value < DateTime.UtcNow)
        {
            vehicle.ReleaseReservation();
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Failure<VehicleStockDto>("Your 15-minute reservation hold has expired. Please reserve again.");
        }

        var orderId = Guid.NewGuid();
        var confirmed = vehicle.ConfirmBooking(buyerId, orderId);

        if (!confirmed)
        {
            return Result.Failure<VehicleStockDto>("Failed to confirm vehicle booking. Reservation state is invalid.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure<VehicleStockDto>("Booking conflict detected. The vehicle was modified by another operation.");
        }

        return Result.Success(MapToDto(vehicle, true));
    }

    private static VehicleStockDto MapToDto(Domain.Entities.VehicleStock vehicle, bool isHeld)
    {
        return new VehicleStockDto(
            vehicle.Id,
            vehicle.CompanyId,
            vehicle.BranchId,
            vehicle.VehicleVariantId,
            vehicle.VinNumber,
            vehicle.EngineNumber,
            vehicle.Color,
            vehicle.Status.ToString(),
            vehicle.Branch?.Name,
            vehicle.VehicleVariant?.VariantName,
            vehicle.VehicleVariant?.VehicleModel?.Make,
            vehicle.VehicleVariant?.VehicleModel?.Model,
            vehicle.VehicleVariant?.ExShowroomPrice ?? 0,
            isHeld,
            vehicle.ReservationExpiresAt
        );
    }
}
