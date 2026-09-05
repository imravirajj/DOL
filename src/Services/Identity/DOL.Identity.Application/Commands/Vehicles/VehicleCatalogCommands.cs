using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Vehicles;

// ── Vehicle Model Commands ──────────────────────────────────
public record CreateVehicleModelCommand(
    Guid CompanyId,
    string Make,
    string Model,
    int Year,
    string Category = "SUV") : IRequest<Result<Guid>>;

public class CreateVehicleModelCommandValidator : AbstractValidator<CreateVehicleModelCommand>
{
    public CreateVehicleModelCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Make).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Year).InclusiveBetween(2000, 2030);
    }
}

public class CreateVehicleModelCommandHandler : IRequestHandler<CreateVehicleModelCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public CreateVehicleModelCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateVehicleModelCommand request, CancellationToken cancellationToken)
    {
        var vehicleModel = new VehicleModel(request.CompanyId, request.Make, request.Model, request.Year, request.Category);
        _context.VehicleModels.Add(vehicleModel);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(vehicleModel.Id);
    }
}

public record UpdateVehicleModelCommand(
    Guid Id,
    string Make,
    string Model,
    int Year,
    string Category) : IRequest<Result>;

public class UpdateVehicleModelCommandHandler : IRequestHandler<UpdateVehicleModelCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateVehicleModelCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateVehicleModelCommand request, CancellationToken cancellationToken)
    {
        var model = await _context.VehicleModels.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (model == null) return Result.Failure("Vehicle Model not found.");

        model.UpdateDetails(request.Make, request.Model, request.Year, request.Category);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteVehicleModelCommand(Guid Id) : IRequest<Result>;

public class DeleteVehicleModelCommandHandler : IRequestHandler<DeleteVehicleModelCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public DeleteVehicleModelCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteVehicleModelCommand request, CancellationToken cancellationToken)
    {
        var model = await _context.VehicleModels.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (model == null) return Result.Failure("Vehicle Model not found.");

        model.SetActiveStatus(false);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Vehicle Variant Commands ────────────────────────────────
public record CreateVehicleVariantCommand(
    Guid CompanyId,
    Guid VehicleModelId,
    string VariantName,
    string FuelType,
    string Transmission,
    decimal ExShowroomPrice,
    string ColorsAvailable) : IRequest<Result<Guid>>;

public class CreateVehicleVariantCommandValidator : AbstractValidator<CreateVehicleVariantCommand>
{
    public CreateVehicleVariantCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.VehicleModelId).NotEmpty();
        RuleFor(x => x.VariantName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ExShowroomPrice).GreaterThan(0);
    }
}

public class CreateVehicleVariantCommandHandler : IRequestHandler<CreateVehicleVariantCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public CreateVehicleVariantCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateVehicleVariantCommand request, CancellationToken cancellationToken)
    {
        var model = await _context.VehicleModels.FirstOrDefaultAsync(m => m.Id == request.VehicleModelId, cancellationToken);
        if (model == null) return Result<Guid>.Failure("Vehicle Model not found.");

        var variant = new VehicleVariant(
            request.CompanyId,
            request.VehicleModelId,
            request.VariantName,
            request.FuelType,
            request.Transmission,
            request.ExShowroomPrice,
            request.ColorsAvailable);

        _context.VehicleVariants.Add(variant);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(variant.Id);
    }
}

public record UpdateVehicleVariantCommand(
    Guid Id,
    string VariantName,
    string FuelType,
    string Transmission,
    decimal ExShowroomPrice,
    string ColorsAvailable) : IRequest<Result>;

public class UpdateVehicleVariantCommandHandler : IRequestHandler<UpdateVehicleVariantCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateVehicleVariantCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateVehicleVariantCommand request, CancellationToken cancellationToken)
    {
        var variant = await _context.VehicleVariants.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);
        if (variant == null) return Result.Failure("Vehicle Variant not found.");

        variant.UpdateDetails(request.VariantName, request.FuelType, request.Transmission, request.ExShowroomPrice, request.ColorsAvailable);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteVehicleVariantCommand(Guid Id) : IRequest<Result>;

public class DeleteVehicleVariantCommandHandler : IRequestHandler<DeleteVehicleVariantCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public DeleteVehicleVariantCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteVehicleVariantCommand request, CancellationToken cancellationToken)
    {
        var variant = await _context.VehicleVariants.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);
        if (variant == null) return Result.Failure("Vehicle Variant not found.");

        variant.SetActiveStatus(false);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
