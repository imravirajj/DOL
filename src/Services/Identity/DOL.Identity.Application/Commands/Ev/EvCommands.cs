using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Ev;

// ── Register Charging Station ───────────────────────────────
public record CreateChargingStationCommand(
    Guid CompanyId,
    string StationName,
    string LocationAddress,
    double Latitude,
    double Longitude,
    string ConnectorType = "CCS2",
    int PowerKw = 60,
    decimal TariffPerKwh = 18.5m,
    Guid? BranchId = null) : IRequest<Result<Guid>>;

public class CreateChargingStationCommandValidator : AbstractValidator<CreateChargingStationCommand>
{
    public CreateChargingStationCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.StationName).NotEmpty();
        RuleFor(x => x.LocationAddress).NotEmpty();
        RuleFor(x => x.PowerKw).GreaterThan(0);
        RuleFor(x => x.TariffPerKwh).GreaterThanOrEqualTo(0);
    }
}

public class CreateChargingStationCommandHandler : IRequestHandler<CreateChargingStationCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public CreateChargingStationCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateChargingStationCommand request, CancellationToken cancellationToken)
    {
        var station = new EvChargingStation(
            request.CompanyId,
            request.StationName,
            request.LocationAddress,
            request.Latitude,
            request.Longitude,
            request.ConnectorType,
            request.PowerKw,
            request.TariffPerKwh,
            request.BranchId);

        _context.EvChargingStations.Add(station);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(station.Id);
    }
}

// ── Request Home Charger Installation ───────────────────────
public record RequestHomeChargerCommand(
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    string InstallationAddress,
    DateTime PreferredSurveyDate,
    string ChargerModel = "7.4 kW AC Fast Home Charger",
    Guid? OrderId = null) : IRequest<Result<Guid>>;

public class RequestHomeChargerCommandValidator : AbstractValidator<RequestHomeChargerCommand>
{
    public RequestHomeChargerCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.InstallationAddress).NotEmpty();
        RuleFor(x => x.PreferredSurveyDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date);
    }
}

public class RequestHomeChargerCommandHandler : IRequestHandler<RequestHomeChargerCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public RequestHomeChargerCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(RequestHomeChargerCommand request, CancellationToken cancellationToken)
    {
        var install = new HomeChargerInstallation(
            request.CompanyId,
            request.BranchId,
            request.BuyerId,
            request.InstallationAddress,
            request.PreferredSurveyDate,
            request.ChargerModel,
            request.OrderId);

        _context.HomeChargerInstallations.Add(install);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(install.Id);
    }
}

// ── Update Home Charger Status ──────────────────────────────
public record UpdateHomeChargerStatusCommand(
    Guid Id,
    HomeChargerSurveyStatus Status,
    string? TechnicianNotes = null) : IRequest<Result<bool>>;

public class UpdateHomeChargerStatusCommandHandler : IRequestHandler<UpdateHomeChargerStatusCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public UpdateHomeChargerStatusCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateHomeChargerStatusCommand request, CancellationToken cancellationToken)
    {
        var install = await _context.HomeChargerInstallations.FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);
        if (install == null) return Result<bool>.Failure("Home charger installation request not found.");

        install.UpdateSurvey(request.Status, request.TechnicianNotes);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

// ── Queries ─────────────────────────────────────────────────
public record GetChargingStationsQuery(string? ConnectorType = null) : IRequest<Result<List<EvChargingStationDto>>>;

public class GetChargingStationsQueryHandler : IRequestHandler<GetChargingStationsQuery, Result<List<EvChargingStationDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetChargingStationsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<EvChargingStationDto>>> Handle(GetChargingStationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.EvChargingStations.AsNoTracking().Where(s => s.IsAvailable);

        if (!string.IsNullOrWhiteSpace(request.ConnectorType))
        {
            var cleanType = request.ConnectorType.Trim().ToUpperInvariant();
            query = query.Where(s => s.ConnectorType == cleanType);
        }

        var list = await query
            .OrderByDescending(s => s.PowerKw)
            .Select(s => new EvChargingStationDto(
                s.Id,
                s.CompanyId,
                s.BranchId,
                s.StationName,
                s.LocationAddress,
                s.Latitude,
                s.Longitude,
                s.ConnectorType,
                s.PowerKw,
                s.TariffPerKwh,
                s.IsAvailable))
            .ToListAsync(cancellationToken);

        return Result<List<EvChargingStationDto>>.Success(list);
    }
}

public record GetHomeChargerByOrderQuery(Guid OrderId) : IRequest<Result<HomeChargerInstallationDto>>;

public class GetHomeChargerByOrderQueryHandler : IRequestHandler<GetHomeChargerByOrderQuery, Result<HomeChargerInstallationDto>>
{
    private readonly IIdentityDbContext _context;

    public GetHomeChargerByOrderQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<HomeChargerInstallationDto>> Handle(GetHomeChargerByOrderQuery request, CancellationToken cancellationToken)
    {
        var h = await _context.HomeChargerInstallations.AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == request.OrderId, cancellationToken);
        if (h == null) return Result<HomeChargerInstallationDto>.Failure("No home charger request found for this order.");

        return Result<HomeChargerInstallationDto>.Success(new HomeChargerInstallationDto(
            h.Id,
            h.CompanyId,
            h.BranchId,
            h.BuyerId,
            h.OrderId,
            h.InstallationAddress,
            h.PreferredSurveyDate,
            h.ChargerModel,
            h.SurveyStatus,
            h.TechnicianNotes,
            h.InstalledAt,
            h.CreatedAt));
    }
}
