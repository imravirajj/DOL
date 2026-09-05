using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Queries.Inventory;

public record GetVehicleStockByIdQuery(Guid Id) : IRequest<Result<VehicleStockDto>>;

public class GetVehicleStockByIdQueryHandler : IRequestHandler<GetVehicleStockByIdQuery, Result<VehicleStockDto>>
{
    private readonly IIdentityDbContext _context;

    public GetVehicleStockByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<VehicleStockDto>> Handle(GetVehicleStockByIdQuery request, CancellationToken cancellationToken)
    {
        var stock = await _context.VehicleStocks
            .Include(s => s.Branch)
            .Include(s => s.VehicleVariant)
                .ThenInclude(v => v!.VehicleModel)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (stock == null) return Result<VehicleStockDto>.Failure("Vehicle stock unit not found.");

        var dto = new VehicleStockDto(
            stock.Id,
            stock.CompanyId,
            stock.BranchId,
            stock.VehicleVariantId,
            stock.VinNumber,
            stock.EngineNumber,
            stock.Color,
            stock.Status.ToString(),
            stock.Branch?.Name,
            stock.VehicleVariant?.VariantName,
            stock.VehicleVariant?.VehicleModel?.Make,
            stock.VehicleVariant?.VehicleModel?.Model,
            stock.VehicleVariant?.ExShowroomPrice ?? 0,
            false,
            stock.ReservationExpiresAt);

        return Result<VehicleStockDto>.Success(dto);
    }
}
