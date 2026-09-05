using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Inventory;

public record CancelWaitlistOrBookingCommand(
    Guid? WaitlistId = null,
    Guid? VehicleStockId = null,
    string Reason = "Customer requested 1-click refund"
) : IRequest<Result<string>>;

public class CancelWaitlistOrBookingCommandHandler : IRequestHandler<CancelWaitlistOrBookingCommand, Result<string>>
{
    private readonly IIdentityDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;

    public CancelWaitlistOrBookingCommandHandler(
        IIdentityDbContext context,
        ICurrentUserContext currentUserContext)
    {
        _context = context;
        _currentUserContext = currentUserContext;
    }

    public async Task<Result<string>> Handle(CancelWaitlistOrBookingCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserContext.UserId.HasValue)
        {
            return Result.Failure<string>("Authentication required.");
        }

        var userId = _currentUserContext.UserId.Value;

        // Case 1: Cancel Waitlist Entry
        if (request.WaitlistId.HasValue)
        {
            var entry = await _context.WaitlistEntries
                .FirstOrDefaultAsync(w => w.Id == request.WaitlistId.Value &&
                                         (w.BuyerId == userId || _currentUserContext.IsCompanyAdmin), cancellationToken);

            if (entry == null)
            {
                return Result.Failure<string>("Waitlist entry not found.");
            }

            if (entry.Status == WaitlistStatus.CancelledAndRefunded)
            {
                return Result.Success("Waitlist entry is already cancelled and refunded.");
            }

            entry.CancelAndRefund();
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success($"100% token refund of ₹{entry.TokenAmountPaid:N0} processed successfully to your payment source. You have exited the waitlist.");
        }

        // Case 2: Cancel Vehicle Stock Reservation
        if (request.VehicleStockId.HasValue)
        {
            var vehicle = await _context.VehicleStocks
                .FirstOrDefaultAsync(s => s.Id == request.VehicleStockId.Value &&
                                         (s.ReservedByBuyerId == userId || _currentUserContext.IsCompanyAdmin), cancellationToken);

            if (vehicle == null)
            {
                return Result.Failure<string>("Vehicle stock not found.");
            }

            vehicle.ReleaseReservation();

            // Check if there is any customer waiting in the waitlist for this variant and branch
            var nextInLine = await _context.WaitlistEntries
                .Where(w => w.BranchId == vehicle.BranchId &&
                            w.VehicleVariantId == vehicle.VehicleVariantId &&
                            w.Status == WaitlistStatus.Waiting)
                .OrderBy(w => w.QueuePosition)
                .FirstOrDefaultAsync(cancellationToken);

            if (nextInLine != null)
            {
                // Auto-allocate to next customer in FIFO queue!
                vehicle.TryReserve(nextInLine.BuyerId, TimeSpan.FromHours(24));
                nextInLine.AllocateStock(vehicle.Id);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success("Booking hold cancelled. Vehicle has been re-allocated to the next customer in the queue.");
        }

        return Result.Failure<string>("Either WaitlistId or VehicleStockId must be provided.");
    }
}
