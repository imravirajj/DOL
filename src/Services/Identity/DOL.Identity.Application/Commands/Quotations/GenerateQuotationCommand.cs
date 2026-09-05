using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Events;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Quotations;

public record GenerateQuotationCommand(
    Guid VehicleVariantId,
    Guid BranchId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string SelectedColor,
    bool IncludeZeroDep = true,
    bool IncludeEngineProtect = true,
    bool IncludeReturnToInvoice = false,
    bool IncludeExtendedWarranty = false,
    decimal AccessoriesTotal = 0,
    decimal DiscountAmount = 0
) : IRequest<Result<QuotationDto>>;

public class GenerateQuotationCommandValidator : AbstractValidator<GenerateQuotationCommand>
{
    public GenerateQuotationCommandValidator()
    {
        RuleFor(x => x.VehicleVariantId).NotEmpty().WithMessage("Vehicle variant is required.");
        RuleFor(x => x.BranchId).NotEmpty().WithMessage("Branch is required.");
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(150).WithMessage("Customer name is required.");
        RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
        RuleFor(x => x.CustomerPhone).NotEmpty().WithMessage("Phone number is required.");
        RuleFor(x => x.SelectedColor).NotEmpty().WithMessage("Color is required.");
        RuleFor(x => x.AccessoriesTotal).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0);
    }
}

public class GenerateQuotationCommandHandler : IRequestHandler<GenerateQuotationCommand, Result<QuotationDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;

    public GenerateQuotationCommandHandler(
        IIdentityDbContext context,
        ICurrentUserContext currentUserContext)
    {
        _context = context;
        _currentUserContext = currentUserContext;
    }

    public async Task<Result<QuotationDto>> Handle(GenerateQuotationCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Variant with Model
        var variant = await _context.VehicleVariants
            .Include(v => v.VehicleModel)
            .FirstOrDefaultAsync(v => v.Id == request.VehicleVariantId, cancellationToken);

        if (variant == null)
        {
            return Result.Failure<QuotationDto>("Vehicle variant not found.");
        }

        // 2. Fetch Branch with City, State
        var branch = await _context.Branches
            .Include(b => b.City)
                .ThenInclude(c => c!.StateRegion)
            .FirstOrDefaultAsync(b => b.Id == request.BranchId, cancellationToken);

        if (branch == null)
        {
            return Result.Failure<QuotationDto>("Target dealership branch not found.");
        }

        var exShowroom = variant.ExShowroomPrice;
        var fuelType = variant.FuelType.ToUpperInvariant();
        var stateName = branch.City?.StateRegion?.Name ?? "General";

        // 3. Calculate RTO Road Tax
        // Check if explicit tax slab exists for this state and fuel type, else use standard rates
        var slab = await _context.RtoTaxSlabs
            .FirstOrDefaultAsync(s => s.CompanyId == variant.CompanyId &&
                                     s.StateName == stateName &&
                                     s.FuelType.ToUpper() == fuelType, cancellationToken);

        decimal rtoPercentage;
        if (slab != null)
        {
            rtoPercentage = slab.TaxPercentage;
        }
        else
        {
            // Standard slabs: EV: 2%, Petrol/Hybrid: 10%, Diesel: 12%
            if (fuelType == "EV")
                rtoPercentage = 2.0m;
            else if (fuelType == "DIESEL")
                rtoPercentage = 12.0m;
            else
                rtoPercentage = 10.0m;
        }

        var rtoTaxAmount = Math.Round(exShowroom * (rtoPercentage / 100m), 2);

        // 4. Calculate Insurance: Base 1-Yr OD + 3-Yr TP (~3.5% of Ex-Showroom)
        var insuranceBase = Math.Round(exShowroom * 0.035m, 2);

        // Insurance Add-ons:
        // Zero Dep: 0.8%, Engine Protect: 0.3%, Return-to-Invoice (RTI): 0.4%
        decimal insuranceAddons = 0;
        if (request.IncludeZeroDep)
            insuranceAddons += Math.Round(exShowroom * 0.008m, 2);
        if (request.IncludeEngineProtect)
            insuranceAddons += Math.Round(exShowroom * 0.003m, 2);
        if (request.IncludeReturnToInvoice)
            insuranceAddons += Math.Round(exShowroom * 0.004m, 2);

        // 5. Statutory Charges: FASTag (₹500), TCS (1% if Ex-Showroom >= ₹10,00,000)
        var fastag = 500m;
        var tcsAmount = exShowroom >= 1000000m ? Math.Round(exShowroom * 0.01m, 2) : 0m;

        // 6. Extended Warranty (4th & 5th Year Comprehensive: approx 1.2% of Ex-Showroom)
        var warrantyAmount = request.IncludeExtendedWarranty ? Math.Round(exShowroom * 0.012m, 2) : 0m;

        // 7. Generate Sequential Quotation Number: QTN-2026-{BRANCH_CODE}-{RANDOM}
        var year = DateTime.UtcNow.Year;
        var randomSuffix = Random.Shared.Next(1000, 9999);
        var qtnNumber = $"QTN-{year}-{branch.BranchCode}-{randomSuffix}";

        var buyerId = _currentUserContext.UserId;

        var quotation = new Quotation(
            variant.CompanyId,
            branch.Id,
            variant.Id,
            buyerId,
            qtnNumber,
            request.CustomerName,
            request.CustomerEmail,
            request.CustomerPhone,
            request.SelectedColor,
            exShowroom,
            rtoTaxAmount,
            insuranceBase,
            insuranceAddons,
            fastag,
            tcsAmount,
            request.AccessoriesTotal,
            warrantyAmount,
            request.DiscountAmount,
            request.IncludeZeroDep,
            request.IncludeEngineProtect,
            request.IncludeReturnToInvoice,
            request.IncludeExtendedWarranty,
            null,
            TimeSpan.FromDays(7) // 7 days price lock guarantee
        );

        quotation.AddDomainEvent(new QuotationGeneratedEvent(
            quotation.Id,
            quotation.QuotationNumber,
            quotation.TotalOnRoadPrice,
            quotation.CustomerEmail
        ));

        _context.Quotations.Add(quotation);
        await _context.SaveChangesAsync(cancellationToken);

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
            variant.VehicleModel?.Make ?? "Automobile",
            variant.VehicleModel?.Model ?? "Car",
            variant.VariantName,
            variant.FuelType,
            variant.Transmission,
            branch.Name,
            stateName,
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
