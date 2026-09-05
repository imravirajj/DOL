using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Exchange;

// ── Valuate & Create Trade-In Request ───────────────────────
public record ValuateTradeInCommand(
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    string Make,
    string Model,
    int Year,
    int KilometersDriven,
    string FuelType,
    string Condition,
    bool HasAccidentHistory,
    string? RegistrationNumber = null) : IRequest<Result<VehicleTradeInDto>>;

public class ValuateTradeInCommandValidator : AbstractValidator<ValuateTradeInCommand>
{
    public ValuateTradeInCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.Make).NotEmpty();
        RuleFor(x => x.Model).NotEmpty();
        RuleFor(x => x.Year).InclusiveBetween(2000, DateTime.UtcNow.Year);
        RuleFor(x => x.KilometersDriven).GreaterThanOrEqualTo(0);
    }
}

public class ValuateTradeInCommandHandler : IRequestHandler<ValuateTradeInCommand, Result<VehicleTradeInDto>>
{
    private readonly IIdentityDbContext _context;

    public ValuateTradeInCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<VehicleTradeInDto>> Handle(ValuateTradeInCommand request, CancellationToken cancellationToken)
    {
        // Algorithmic Base Valuation:
        // Base market value assumption for modern Indian passenger vehicle (~8 Lakhs benchmark)
        decimal basePrice = 850_000m;
        int ageYears = Math.Max(0, DateTime.UtcNow.Year - request.Year);
        
        // Depreciation per year: 10% compounding approx
        decimal depreciationFactor = (decimal)Math.Pow(0.88, ageYears);
        decimal value = basePrice * depreciationFactor;

        // Mileage depreciation
        decimal mileageDeduction = (request.KilometersDriven / 10000m) * 15000m;
        value = Math.Max(50000m, value - mileageDeduction);

        // Condition multiplier
        value = request.Condition.ToLowerInvariant() switch
        {
            "excellent" => value * 1.10m,
            "good" => value * 1.00m,
            "fair" => value * 0.85m,
            "poor" => value * 0.70m,
            _ => value
        };

        if (request.HasAccidentHistory)
        {
            value *= 0.75m; // 25% drop for accident history
        }

        decimal estimatedValue = Math.Round(Math.Max(30000m, value), 0);

        var tradeIn = new VehicleTradeIn(
            request.CompanyId,
            request.BranchId,
            request.BuyerId,
            request.Make,
            request.Model,
            request.Year,
            request.KilometersDriven,
            request.FuelType,
            request.Condition,
            request.HasAccidentHistory,
            estimatedValue,
            request.RegistrationNumber);

        _context.VehicleTradeIns.Add(tradeIn);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new VehicleTradeInDto(
            tradeIn.Id,
            tradeIn.CompanyId,
            tradeIn.BranchId,
            tradeIn.BuyerId,
            tradeIn.Make,
            tradeIn.Model,
            tradeIn.Year,
            tradeIn.KilometersDriven,
            tradeIn.FuelType,
            tradeIn.Condition,
            tradeIn.HasAccidentHistory,
            tradeIn.RegistrationNumber,
            tradeIn.EstimatedValue,
            tradeIn.OfferedValue,
            tradeIn.InspectionDate,
            tradeIn.InspectorNotes,
            tradeIn.Status,
            tradeIn.CreatedAt);

        return Result<VehicleTradeInDto>.Success(dto);
    }
}

// ── Schedule Inspection ─────────────────────────────────────
public record ScheduleInspectionCommand(
    Guid TradeInId,
    DateTime InspectionDate) : IRequest<Result<bool>>;

public class ScheduleInspectionCommandHandler : IRequestHandler<ScheduleInspectionCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public ScheduleInspectionCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(ScheduleInspectionCommand request, CancellationToken cancellationToken)
    {
        var tradeIn = await _context.VehicleTradeIns.FirstOrDefaultAsync(t => t.Id == request.TradeInId, cancellationToken);
        if (tradeIn == null) return Result<bool>.Failure("Trade-in record not found.");

        tradeIn.ScheduleInspection(request.InspectionDate);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Provide Final Offer ─────────────────────────────────────
public record ProvideOfferCommand(
    Guid TradeInId,
    decimal OfferedValue,
    string? InspectorNotes = null) : IRequest<Result<bool>>;

public class ProvideOfferCommandHandler : IRequestHandler<ProvideOfferCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public ProvideOfferCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(ProvideOfferCommand request, CancellationToken cancellationToken)
    {
        var tradeIn = await _context.VehicleTradeIns.FirstOrDefaultAsync(t => t.Id == request.TradeInId, cancellationToken);
        if (tradeIn == null) return Result<bool>.Failure("Trade-in record not found.");

        tradeIn.ProvideOffer(request.OfferedValue, request.InspectorNotes);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Respond to Offer (Accept / Reject) ───────────────────────
public record RespondToOfferCommand(
    Guid TradeInId,
    bool Accept) : IRequest<Result<bool>>;

public class RespondToOfferCommandHandler : IRequestHandler<RespondToOfferCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public RespondToOfferCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(RespondToOfferCommand request, CancellationToken cancellationToken)
    {
        var tradeIn = await _context.VehicleTradeIns.FirstOrDefaultAsync(t => t.Id == request.TradeInId, cancellationToken);
        if (tradeIn == null) return Result<bool>.Failure("Trade-in record not found.");

        if (request.Accept) tradeIn.AcceptOffer();
        else tradeIn.RejectOffer();

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Queries & Deletion ──────────────────────────────────────
public record GetTradeInsQuery(Guid? BuyerId = null) : IRequest<Result<List<VehicleTradeInDto>>>;

public class GetTradeInsQueryHandler : IRequestHandler<GetTradeInsQuery, Result<List<VehicleTradeInDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetTradeInsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<VehicleTradeInDto>>> Handle(GetTradeInsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VehicleTradeIns.AsNoTracking();
        if (request.BuyerId.HasValue)
        {
            query = query.Where(t => t.BuyerId == request.BuyerId.Value);
        }

        var list = await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new VehicleTradeInDto(
                t.Id,
                t.CompanyId,
                t.BranchId,
                t.BuyerId,
                t.Make,
                t.Model,
                t.Year,
                t.KilometersDriven,
                t.FuelType,
                t.Condition,
                t.HasAccidentHistory,
                t.RegistrationNumber,
                t.EstimatedValue,
                t.OfferedValue,
                t.InspectionDate,
                t.InspectorNotes,
                t.Status,
                t.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<VehicleTradeInDto>>.Success(list);
    }
}

public record GetTradeInByIdQuery(Guid Id) : IRequest<Result<VehicleTradeInDto>>;

public class GetTradeInByIdQueryHandler : IRequestHandler<GetTradeInByIdQuery, Result<VehicleTradeInDto>>
{
    private readonly IIdentityDbContext _context;

    public GetTradeInByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<VehicleTradeInDto>> Handle(GetTradeInByIdQuery request, CancellationToken cancellationToken)
    {
        var t = await _context.VehicleTradeIns.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (t == null) return Result<VehicleTradeInDto>.Failure("Trade-in request not found.");

        return Result<VehicleTradeInDto>.Success(new VehicleTradeInDto(
            t.Id,
            t.CompanyId,
            t.BranchId,
            t.BuyerId,
            t.Make,
            t.Model,
            t.Year,
            t.KilometersDriven,
            t.FuelType,
            t.Condition,
            t.HasAccidentHistory,
            t.RegistrationNumber,
            t.EstimatedValue,
            t.OfferedValue,
            t.InspectionDate,
            t.InspectorNotes,
            t.Status,
            t.CreatedAt));
    }
}

public record DeleteTradeInCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteTradeInCommandHandler : IRequestHandler<DeleteTradeInCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public DeleteTradeInCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteTradeInCommand request, CancellationToken cancellationToken)
    {
        var tradeIn = await _context.VehicleTradeIns.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (tradeIn == null) return Result<bool>.Failure("Trade-in record not found.");

        _context.VehicleTradeIns.Remove(tradeIn);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
