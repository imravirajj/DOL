using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Queries.Vehicles;

// ── Model Queries ───────────────────────────────────────────
public record GetVehicleModelsQuery(
    string? Category = null,
    bool? ActiveOnly = true) : IRequest<Result<List<VehicleModelDto>>>;

public class GetVehicleModelsQueryHandler : IRequestHandler<GetVehicleModelsQuery, Result<List<VehicleModelDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetVehicleModelsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<VehicleModelDto>>> Handle(GetVehicleModelsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VehicleModels
            .Include(m => m.Variants)
            .AsNoTracking();

        if (request.ActiveOnly.HasValue && request.ActiveOnly.Value)
        {
            query = query.Where(m => m.IsActive);
        }

        if (!string.IsNullOrEmpty(request.Category))
        {
            query = query.Where(m => m.Category == request.Category);
        }

        var models = await query
            .OrderBy(m => m.Make)
            .ThenBy(m => m.Model)
            .Select(m => new VehicleModelDto(
                m.Id,
                m.CompanyId,
                m.Make,
                m.Model,
                m.Year,
                m.Category,
                m.IsActive,
                m.Variants.Count,
                m.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<VehicleModelDto>>.Success(models);
    }
}

public record GetVehicleModelByIdQuery(Guid Id) : IRequest<Result<VehicleModelDto>>;

public class GetVehicleModelByIdQueryHandler : IRequestHandler<GetVehicleModelByIdQuery, Result<VehicleModelDto>>
{
    private readonly IIdentityDbContext _context;

    public GetVehicleModelByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<VehicleModelDto>> Handle(GetVehicleModelByIdQuery request, CancellationToken cancellationToken)
    {
        var model = await _context.VehicleModels
            .Include(m => m.Variants)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (model == null) return Result<VehicleModelDto>.Failure("Vehicle Model not found.");

        var dto = new VehicleModelDto(
            model.Id,
            model.CompanyId,
            model.Make,
            model.Model,
            model.Year,
            model.Category,
            model.IsActive,
            model.Variants.Count,
            model.CreatedAt);

        return Result<VehicleModelDto>.Success(dto);
    }
}

// ── Variant Queries ─────────────────────────────────────────
public record GetVehicleVariantsQuery(
    Guid? VehicleModelId = null,
    bool? ActiveOnly = true) : IRequest<Result<List<VehicleVariantDto>>>;

public class GetVehicleVariantsQueryHandler : IRequestHandler<GetVehicleVariantsQuery, Result<List<VehicleVariantDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetVehicleVariantsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<VehicleVariantDto>>> Handle(GetVehicleVariantsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VehicleVariants
            .Include(v => v.VehicleModel)
            .Include(v => v.StockUnits)
            .AsNoTracking();

        if (request.VehicleModelId.HasValue)
        {
            query = query.Where(v => v.VehicleModelId == request.VehicleModelId.Value);
        }

        if (request.ActiveOnly.HasValue && request.ActiveOnly.Value)
        {
            query = query.Where(v => v.IsActive);
        }

        var variants = await query
            .OrderBy(v => v.VariantName)
            .Select(v => new VehicleVariantDto(
                v.Id,
                v.CompanyId,
                v.VehicleModelId,
                v.VehicleModel != null ? $"{v.VehicleModel.Make} {v.VehicleModel.Model}" : string.Empty,
                v.VariantName,
                v.FuelType,
                v.Transmission,
                v.ExShowroomPrice,
                v.ColorsAvailable,
                v.IsActive,
                v.StockUnits.Count,
                v.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<VehicleVariantDto>>.Success(variants);
    }
}

public record GetVehicleVariantByIdQuery(Guid Id) : IRequest<Result<VehicleVariantDto>>;

public class GetVehicleVariantByIdQueryHandler : IRequestHandler<GetVehicleVariantByIdQuery, Result<VehicleVariantDto>>
{
    private readonly IIdentityDbContext _context;

    public GetVehicleVariantByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<VehicleVariantDto>> Handle(GetVehicleVariantByIdQuery request, CancellationToken cancellationToken)
    {
        var variant = await _context.VehicleVariants
            .Include(v => v.VehicleModel)
            .Include(v => v.StockUnits)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (variant == null) return Result<VehicleVariantDto>.Failure("Vehicle Variant not found.");

        var dto = new VehicleVariantDto(
            variant.Id,
            variant.CompanyId,
            variant.VehicleModelId,
            variant.VehicleModel != null ? $"{variant.VehicleModel.Make} {variant.VehicleModel.Model}" : string.Empty,
            variant.VariantName,
            variant.FuelType,
            variant.Transmission,
            variant.ExShowroomPrice,
            variant.ColorsAvailable,
            variant.IsActive,
            variant.StockUnits.Count,
            variant.CreatedAt);

        return Result<VehicleVariantDto>.Success(dto);
    }
}
