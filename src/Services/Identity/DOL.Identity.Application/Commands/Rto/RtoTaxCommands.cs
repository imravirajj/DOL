using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Rto;

public record CreateRtoTaxSlabCommand(
    Guid CompanyId,
    string StateName,
    string FuelType,
    decimal TaxPercentage,
    decimal CessPercentage = 0) : IRequest<Result<Guid>>;

public class CreateRtoTaxSlabCommandValidator : AbstractValidator<CreateRtoTaxSlabCommand>
{
    public CreateRtoTaxSlabCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.StateName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.FuelType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TaxPercentage).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CessPercentage).GreaterThanOrEqualTo(0);
    }
}

public class CreateRtoTaxSlabCommandHandler : IRequestHandler<CreateRtoTaxSlabCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public CreateRtoTaxSlabCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateRtoTaxSlabCommand request, CancellationToken cancellationToken)
    {
        var slab = new RtoTaxSlab(request.CompanyId, request.StateName, request.FuelType, request.TaxPercentage, request.CessPercentage);
        _context.RtoTaxSlabs.Add(slab);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(slab.Id);
    }
}

public record UpdateRtoTaxSlabCommand(
    Guid Id,
    decimal TaxPercentage,
    decimal CessPercentage) : IRequest<Result>;

public class UpdateRtoTaxSlabCommandHandler : IRequestHandler<UpdateRtoTaxSlabCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateRtoTaxSlabCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateRtoTaxSlabCommand request, CancellationToken cancellationToken)
    {
        var slab = await _context.RtoTaxSlabs.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (slab == null) return Result.Failure("RTO Tax Slab not found.");

        slab.Update(request.TaxPercentage, request.CessPercentage);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteRtoTaxSlabCommand(Guid Id) : IRequest<Result>;

public class DeleteRtoTaxSlabCommandHandler : IRequestHandler<DeleteRtoTaxSlabCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public DeleteRtoTaxSlabCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteRtoTaxSlabCommand request, CancellationToken cancellationToken)
    {
        var slab = await _context.RtoTaxSlabs.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (slab == null) return Result.Failure("RTO Tax Slab not found.");

        _context.RtoTaxSlabs.Remove(slab);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
