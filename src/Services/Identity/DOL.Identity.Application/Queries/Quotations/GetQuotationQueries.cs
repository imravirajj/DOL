using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Queries.Quotations;

public record GetQuotationByIdQuery(Guid Id) : IRequest<Result<QuotationDto>>;

public class GetQuotationByIdQueryHandler : IRequestHandler<GetQuotationByIdQuery, Result<QuotationDto>>
{
    private readonly IIdentityDbContext _context;

    public GetQuotationByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<QuotationDto>> Handle(GetQuotationByIdQuery request, CancellationToken cancellationToken)
    {
        var quotation = await _context.Quotations
            .Include(q => q.Branch)
                .ThenInclude(b => b!.City)
                    .ThenInclude(c => c!.StateRegion)
            .Include(q => q.VehicleVariant)
                .ThenInclude(v => v!.VehicleModel)
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);

        if (quotation == null)
        {
            return Result.Failure<QuotationDto>("Quotation not found.");
        }

        var dto = new QuotationDto(
            quotation.Id,
            quotation.CompanyId,
            quotation.BranchId,
            quotation.VehicleVariantId,
            quotation.BuyerId,
            quotation.QuotationNumber,
            quotation.CustomerName,
            quotation.CustomerEmail,
            quotation.CustomerPhone,
            quotation.SelectedColor,
            quotation.VehicleVariant?.VehicleModel?.Make ?? "Automobile",
            quotation.VehicleVariant?.VehicleModel?.Model ?? "Car",
            quotation.VehicleVariant?.VariantName ?? "Variant",
            quotation.VehicleVariant?.FuelType ?? "Petrol",
            quotation.VehicleVariant?.Transmission ?? "Manual",
            quotation.Branch?.Name ?? "Branch",
            quotation.Branch?.City?.StateRegion?.Name ?? "General",
            quotation.ExShowroomPrice,
            quotation.RtoTaxAmount,
            quotation.InsuranceBaseAmount,
            quotation.InsuranceAddonsAmount,
            quotation.FastagCharges,
            quotation.TcsAmount,
            quotation.AccessoriesTotal,
            quotation.ExtendedWarrantyAmount,
            quotation.DiscountAmount,
            quotation.TotalOnRoadPrice,
            quotation.IncludeZeroDep,
            quotation.IncludeEngineProtect,
            quotation.IncludeReturnToInvoice,
            quotation.IncludeExtendedWarranty,
            quotation.Status.ToString(),
            quotation.ValidUntil,
            quotation.CreatedAt
        );

        return Result.Success(dto);
    }
}

public record GetQuotationsQuery(
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<List<QuotationSummaryDto>>>;

public class GetQuotationsQueryHandler : IRequestHandler<GetQuotationsQuery, Result<List<QuotationSummaryDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetQuotationsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<QuotationSummaryDto>>> Handle(GetQuotationsQuery request, CancellationToken cancellationToken)
    {
        var quotations = await _context.Quotations
            .Include(q => q.VehicleVariant)
                .ThenInclude(v => v!.VehicleModel)
            .AsNoTracking()
            .OrderByDescending(q => q.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(q => new QuotationSummaryDto(
                q.Id,
                q.QuotationNumber,
                q.CustomerName,
                q.VehicleVariant != null && q.VehicleVariant.VehicleModel != null ? q.VehicleVariant.VehicleModel.Make : "",
                q.VehicleVariant != null && q.VehicleVariant.VehicleModel != null ? q.VehicleVariant.VehicleModel.Model : "",
                q.VehicleVariant != null ? q.VehicleVariant.VariantName : "",
                q.TotalOnRoadPrice,
                q.Status.ToString(),
                q.ValidUntil,
                q.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(quotations);
    }
}
