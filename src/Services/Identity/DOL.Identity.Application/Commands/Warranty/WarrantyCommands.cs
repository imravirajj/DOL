using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Warranty;

// ── Create Warranty Package ─────────────────────────────────
public record CreateWarrantyPackageCommand(
    Guid CompanyId,
    string Name,
    WarrantyPackageType PackageType,
    int DurationMonths,
    int KilometerLimit,
    decimal Price,
    string Description) : IRequest<Result<Guid>>;

public class CreateWarrantyPackageCommandValidator : AbstractValidator<CreateWarrantyPackageCommand>
{
    public CreateWarrantyPackageCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.DurationMonths).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}

public class CreateWarrantyPackageCommandHandler : IRequestHandler<CreateWarrantyPackageCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public CreateWarrantyPackageCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateWarrantyPackageCommand request, CancellationToken cancellationToken)
    {
        var pkg = new WarrantyPackage(
            request.CompanyId,
            request.Name,
            request.PackageType,
            request.DurationMonths,
            request.KilometerLimit,
            request.Price,
            request.Description);

        _context.WarrantyPackages.Add(pkg);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(pkg.Id);
    }
}

// ── Subscribe Warranty for Vehicle ──────────────────────────
public record SubscribeWarrantyCommand(
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    Guid WarrantyPackageId,
    string VinNumber,
    Guid? OrderId = null) : IRequest<Result<Guid>>;

public class SubscribeWarrantyCommandHandler : IRequestHandler<SubscribeWarrantyCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public SubscribeWarrantyCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(SubscribeWarrantyCommand request, CancellationToken cancellationToken)
    {
        var pkg = await _context.WarrantyPackages.FirstOrDefaultAsync(p => p.Id == request.WarrantyPackageId, cancellationToken);
        if (pkg == null) return Result<Guid>.Failure("Warranty package not found.");

        string subNo = $"WRN-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddMonths(pkg.DurationMonths);

        var sub = new VehicleWarrantySubscription(
            request.CompanyId,
            request.BranchId,
            request.BuyerId,
            request.WarrantyPackageId,
            request.VinNumber,
            subNo,
            startDate,
            endDate,
            pkg.Price,
            request.OrderId);

        _context.VehicleWarrantySubscriptions.Add(sub);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(sub.Id);
    }
}

// ── Cancel Subscription ─────────────────────────────────────
public record CancelWarrantySubscriptionCommand(Guid Id) : IRequest<Result<bool>>;

public class CancelWarrantySubscriptionCommandHandler : IRequestHandler<CancelWarrantySubscriptionCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public CancelWarrantySubscriptionCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(CancelWarrantySubscriptionCommand request, CancellationToken cancellationToken)
    {
        var sub = await _context.VehicleWarrantySubscriptions.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (sub == null) return Result<bool>.Failure("Warranty subscription not found.");

        sub.Cancel();
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Queries ─────────────────────────────────────────────────
public record GetWarrantyPackagesQuery(WarrantyPackageType? PackageType = null) : IRequest<Result<List<WarrantyPackageDto>>>;

public class GetWarrantyPackagesQueryHandler : IRequestHandler<GetWarrantyPackagesQuery, Result<List<WarrantyPackageDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetWarrantyPackagesQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<WarrantyPackageDto>>> Handle(GetWarrantyPackagesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.WarrantyPackages.AsNoTracking().Where(p => p.IsActive);

        if (request.PackageType.HasValue)
        {
            query = query.Where(p => p.PackageType == request.PackageType.Value);
        }

        var list = await query
            .OrderBy(p => p.Price)
            .Select(p => new WarrantyPackageDto(
                p.Id,
                p.CompanyId,
                p.Name,
                p.PackageType,
                p.DurationMonths,
                p.KilometerLimit,
                p.Price,
                p.Description,
                p.IsActive))
            .ToListAsync(cancellationToken);

        return Result<List<WarrantyPackageDto>>.Success(list);
    }
}

public record GetMyWarrantySubscriptionsQuery(Guid? BuyerId = null, string? Vin = null) : IRequest<Result<List<VehicleWarrantySubscriptionDto>>>;

public class GetMyWarrantySubscriptionsQueryHandler : IRequestHandler<GetMyWarrantySubscriptionsQuery, Result<List<VehicleWarrantySubscriptionDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetMyWarrantySubscriptionsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<VehicleWarrantySubscriptionDto>>> Handle(GetMyWarrantySubscriptionsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VehicleWarrantySubscriptions.AsNoTracking();

        if (request.BuyerId.HasValue)
        {
            query = query.Where(s => s.BuyerId == request.BuyerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Vin))
        {
            var cleanVin = request.Vin.Trim().ToUpperInvariant();
            query = query.Where(s => s.VinNumber == cleanVin);
        }

        var list = await query
            .OrderByDescending(s => s.StartDate)
            .Select(s => new VehicleWarrantySubscriptionDto(
                s.Id,
                s.CompanyId,
                s.BranchId,
                s.BuyerId,
                s.OrderId,
                s.WarrantyPackageId,
                s.VinNumber,
                s.SubscriptionNumber,
                s.StartDate,
                s.EndDate,
                s.PricePaid,
                s.Status,
                s.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<VehicleWarrantySubscriptionDto>>.Success(list);
    }
}

public record GetWarrantySubscriptionByIdQuery(Guid Id) : IRequest<Result<VehicleWarrantySubscriptionDto>>;

public class GetWarrantySubscriptionByIdQueryHandler : IRequestHandler<GetWarrantySubscriptionByIdQuery, Result<VehicleWarrantySubscriptionDto>>
{
    private readonly IIdentityDbContext _context;

    public GetWarrantySubscriptionByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<VehicleWarrantySubscriptionDto>> Handle(GetWarrantySubscriptionByIdQuery request, CancellationToken cancellationToken)
    {
        var s = await _context.VehicleWarrantySubscriptions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (s == null) return Result<VehicleWarrantySubscriptionDto>.Failure("Warranty subscription not found.");

        return Result<VehicleWarrantySubscriptionDto>.Success(new VehicleWarrantySubscriptionDto(
            s.Id,
            s.CompanyId,
            s.BranchId,
            s.BuyerId,
            s.OrderId,
            s.WarrantyPackageId,
            s.VinNumber,
            s.SubscriptionNumber,
            s.StartDate,
            s.EndDate,
            s.PricePaid,
            s.Status,
            s.CreatedAt));
    }
}
