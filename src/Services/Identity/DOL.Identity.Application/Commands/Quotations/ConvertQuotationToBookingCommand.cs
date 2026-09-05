using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Quotations;

public record ConvertQuotationToBookingCommand(
    Guid QuotationId
) : IRequest<Result<ReservationResultDto>>;

public class ConvertQuotationToBookingCommandHandler : IRequestHandler<ConvertQuotationToBookingCommand, Result<ReservationResultDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;

    public ConvertQuotationToBookingCommandHandler(
        IIdentityDbContext context,
        ICurrentUserContext currentUserContext)
    {
        _context = context;
        _currentUserContext = currentUserContext;
    }

    public async Task<Result<ReservationResultDto>> Handle(ConvertQuotationToBookingCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserContext.UserId.HasValue)
        {
            return Result.Failure<ReservationResultDto>("Authentication required to book from quotation.");
        }

        var buyerId = _currentUserContext.UserId.Value;

        var quotation = await _context.Quotations
            .FirstOrDefaultAsync(q => q.Id == request.QuotationId, cancellationToken);

        if (quotation == null)
        {
            return Result.Failure<ReservationResultDto>("Quotation not found.");
        }

        if (quotation.ValidUntil < DateTime.UtcNow)
        {
            quotation.Expire();
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Failure<ReservationResultDto>("This quotation has expired. Please generate a fresh quotation.");
        }

        var now = DateTime.UtcNow;

        // Find available stock for this variant in the branch
        var availableStock = await _context.VehicleStocks
            .FirstOrDefaultAsync(s => s.BranchId == quotation.BranchId &&
                                     s.VehicleVariantId == quotation.VehicleVariantId &&
                                     (s.Status == VehicleStockStatus.Available ||
                                     (s.Status == VehicleStockStatus.Reserved && s.ReservationExpiresAt < now)),
                                 cancellationToken);

        if (availableStock == null)
        {
            return Result.Failure<ReservationResultDto>(
                "Car is currently out of stock at this branch. Please join the priority waitlist or check nearby branches."
            );
        }

        // Lock vehicle for 15 minutes
        var reserved = availableStock.TryReserve(buyerId, TimeSpan.FromMinutes(15));
        if (!reserved)
        {
            return Result.Failure<ReservationResultDto>("The vehicle is currently being reserved by another buyer. Please try again or join the waitlist.");
        }

        quotation.MarkConvertedToBooking();
        await _context.SaveChangesAsync(cancellationToken);

        var expiresAt = availableStock.ReservationExpiresAt ?? now.AddMinutes(15);
        var remainingSeconds = (int)(expiresAt - now).TotalSeconds;

        return Result.Success(new ReservationResultDto(
            true,
            availableStock.Id,
            availableStock.VinNumber,
            expiresAt,
            $"Quotation {quotation.QuotationNumber} accepted! Vehicle locked for 15 minutes. Please complete payment.",
            remainingSeconds
        ));
    }
}
