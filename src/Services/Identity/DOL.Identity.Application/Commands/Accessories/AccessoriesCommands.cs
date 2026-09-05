using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Accessories;

// ── Create Accessory ────────────────────────────────────────
public record CreateAccessoryCommand(
    Guid CompanyId,
    string Name,
    string PartNumber,
    AccessoryCategory Category,
    decimal Price,
    decimal InstallationCost = 0,
    int WarrantyMonths = 12,
    Guid? CompatibleVariantId = null) : IRequest<Result<Guid>>;

public class CreateAccessoryCommandValidator : AbstractValidator<CreateAccessoryCommand>
{
    public CreateAccessoryCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.PartNumber).NotEmpty();
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}

public class CreateAccessoryCommandHandler : IRequestHandler<CreateAccessoryCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public CreateAccessoryCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateAccessoryCommand request, CancellationToken cancellationToken)
    {
        var existing = await _context.VehicleAccessories
            .AnyAsync(a => a.CompanyId == request.CompanyId && a.PartNumber == request.PartNumber.Trim().ToUpperInvariant(), cancellationToken);
        if (existing) return Result<Guid>.Failure("An accessory with this part number already exists.");

        var accessory = new VehicleAccessory(
            request.CompanyId,
            request.Name,
            request.PartNumber,
            request.Category,
            request.Price,
            request.InstallationCost,
            request.WarrantyMonths,
            request.CompatibleVariantId);

        _context.VehicleAccessories.Add(accessory);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(accessory.Id);
    }
}

// ── Update Accessory ────────────────────────────────────────
public record UpdateAccessoryCommand(
    Guid Id,
    string Name,
    AccessoryCategory Category,
    decimal Price,
    decimal InstallationCost,
    int WarrantyMonths,
    Guid? CompatibleVariantId,
    bool IsActive) : IRequest<Result<bool>>;

public class UpdateAccessoryCommandHandler : IRequestHandler<UpdateAccessoryCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public UpdateAccessoryCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateAccessoryCommand request, CancellationToken cancellationToken)
    {
        var accessory = await _context.VehicleAccessories.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (accessory == null) return Result<bool>.Failure("Accessory not found.");

        accessory.Update(
            request.Name,
            request.Category,
            request.Price,
            request.InstallationCost,
            request.WarrantyMonths,
            request.CompatibleVariantId,
            request.IsActive);

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Delete Accessory ────────────────────────────────────────
public record DeleteAccessoryCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteAccessoryCommandHandler : IRequestHandler<DeleteAccessoryCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public DeleteAccessoryCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteAccessoryCommand request, CancellationToken cancellationToken)
    {
        var accessory = await _context.VehicleAccessories.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (accessory == null) return Result<bool>.Failure("Accessory not found.");

        _context.VehicleAccessories.Remove(accessory);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Queries ─────────────────────────────────────────────────
public record GetAccessoriesQuery(
    Guid? CompatibleVariantId = null,
    AccessoryCategory? Category = null) : IRequest<Result<List<VehicleAccessoryDto>>>;

public class GetAccessoriesQueryHandler : IRequestHandler<GetAccessoriesQuery, Result<List<VehicleAccessoryDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetAccessoriesQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<VehicleAccessoryDto>>> Handle(GetAccessoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VehicleAccessories.AsNoTracking().Where(a => a.IsActive);

        if (request.CompatibleVariantId.HasValue)
        {
            // Return accessories compatible with this variant OR universal accessories (CompatibleVariantId == null)
            query = query.Where(a => a.CompatibleVariantId == null || a.CompatibleVariantId == request.CompatibleVariantId.Value);
        }

        if (request.Category.HasValue)
        {
            query = query.Where(a => a.Category == request.Category.Value);
        }

        var list = await query
            .OrderBy(a => a.Category)
            .ThenBy(a => a.Name)
            .Select(a => new VehicleAccessoryDto(
                a.Id,
                a.CompanyId,
                a.Name,
                a.PartNumber,
                a.Category,
                a.CompatibleVariantId,
                a.Price,
                a.InstallationCost,
                a.WarrantyMonths,
                a.IsActive))
            .ToListAsync(cancellationToken);

        return Result<List<VehicleAccessoryDto>>.Success(list);
    }
}

public record GetAccessoryByIdQuery(Guid Id) : IRequest<Result<VehicleAccessoryDto>>;

public class GetAccessoryByIdQueryHandler : IRequestHandler<GetAccessoryByIdQuery, Result<VehicleAccessoryDto>>
{
    private readonly IIdentityDbContext _context;

    public GetAccessoryByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<VehicleAccessoryDto>> Handle(GetAccessoryByIdQuery request, CancellationToken cancellationToken)
    {
        var a = await _context.VehicleAccessories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (a == null) return Result<VehicleAccessoryDto>.Failure("Accessory not found.");

        return Result<VehicleAccessoryDto>.Success(new VehicleAccessoryDto(
            a.Id,
            a.CompanyId,
            a.Name,
            a.PartNumber,
            a.Category,
            a.CompatibleVariantId,
            a.Price,
            a.InstallationCost,
            a.WarrantyMonths,
            a.IsActive));
    }
}
