using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Inventory;

public record AddVehicleStockCommand(
    Guid CompanyId,
    Guid BranchId,
    Guid VehicleVariantId,
    string VinNumber,
    string EngineNumber,
    string Color) : IRequest<Result<Guid>>;

public class AddVehicleStockCommandValidator : AbstractValidator<AddVehicleStockCommand>
{
    public AddVehicleStockCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.VehicleVariantId).NotEmpty();
        RuleFor(x => x.VinNumber).NotEmpty().Length(17).WithMessage("VIN must be exactly 17 characters.");
        RuleFor(x => x.EngineNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(50);
    }
}

public class AddVehicleStockCommandHandler : IRequestHandler<AddVehicleStockCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public AddVehicleStockCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(AddVehicleStockCommand request, CancellationToken cancellationToken)
    {
        var existingVin = await _context.VehicleStocks
            .AnyAsync(s => s.VinNumber == request.VinNumber.Trim().ToUpperInvariant(), cancellationToken);

        if (existingVin)
        {
            return Result<Guid>.Failure($"Vehicle with VIN '{request.VinNumber.ToUpperInvariant()}' already exists in inventory.");
        }

        var stock = new VehicleStock(
            request.CompanyId,
            request.BranchId,
            request.VehicleVariantId,
            request.VinNumber,
            request.EngineNumber,
            request.Color);

        _context.VehicleStocks.Add(stock);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(stock.Id);
    }
}

public record UpdateVehicleStockCommand(
    Guid Id,
    string Color,
    string EngineNumber,
    VehicleStockStatus Status) : IRequest<Result>;

public class UpdateVehicleStockCommandHandler : IRequestHandler<UpdateVehicleStockCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateVehicleStockCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateVehicleStockCommand request, CancellationToken cancellationToken)
    {
        var stock = await _context.VehicleStocks.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (stock == null) return Result.Failure("Vehicle stock not found.");

        stock.UpdateDetails(request.Color, request.EngineNumber);
        stock.SetStatus(request.Status);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public record DeleteVehicleStockCommand(Guid Id) : IRequest<Result>;

public class DeleteVehicleStockCommandHandler : IRequestHandler<DeleteVehicleStockCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public DeleteVehicleStockCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteVehicleStockCommand request, CancellationToken cancellationToken)
    {
        var stock = await _context.VehicleStocks.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (stock == null) return Result.Failure("Vehicle stock not found.");

        _context.VehicleStocks.Remove(stock);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
