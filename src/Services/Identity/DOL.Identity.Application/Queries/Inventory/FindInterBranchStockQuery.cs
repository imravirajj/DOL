using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Queries.Inventory;

public record FindInterBranchStockQuery(
    Guid VehicleVariantId,
    Guid CurrentBranchId
) : IRequest<Result<List<InterBranchStockDto>>>;

public class FindInterBranchStockQueryHandler : IRequestHandler<FindInterBranchStockQuery, Result<List<InterBranchStockDto>>>
{
    private readonly IIdentityDbContext _context;

    public FindInterBranchStockQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<InterBranchStockDto>>> Handle(FindInterBranchStockQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        // Query available stock across other branches of the company (ignoring branch-level filter for search)
        var siblingStock = await _context.VehicleStocks
            .IgnoreQueryFilters()
            .Include(s => s.Branch)
                .ThenInclude(b => b!.City)
            .Include(s => s.VehicleVariant)
            .Where(s => s.VehicleVariantId == request.VehicleVariantId &&
                        s.BranchId != request.CurrentBranchId &&
                        (s.Status == VehicleStockStatus.Available ||
                        (s.Status == VehicleStockStatus.Reserved && s.ReservationExpiresAt < now)))
            .OrderBy(s => s.Branch!.City!.Name)
            .Select(s => new InterBranchStockDto(
                s.Id,
                s.BranchId,
                s.Branch != null ? s.Branch.Name : "Other Branch",
                s.Branch != null && s.Branch.City != null ? s.Branch.City.Name : "Same Region",
                s.VinNumber,
                s.Color,
                s.VehicleVariant != null ? s.VehicleVariant.ExShowroomPrice : 0,
                "2 business days (Intra-city/Regional transfer)"
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(siblingStock);
    }
}
