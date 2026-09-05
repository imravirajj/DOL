using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Service;

// ── Book Service Appointment ────────────────────────────────
public record BookServiceAppointmentCommand(
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    string VinNumber,
    string RegistrationNumber,
    ServiceType ServiceType,
    DateTime AppointmentDate,
    string TimeSlot,
    decimal EstimatedCost = 1500,
    string? CustomerComments = null,
    Guid? VehicleVariantId = null) : IRequest<Result<Guid>>;

public class BookServiceAppointmentCommandValidator : AbstractValidator<BookServiceAppointmentCommand>
{
    public BookServiceAppointmentCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.VinNumber).NotEmpty();
        RuleFor(x => x.RegistrationNumber).NotEmpty();
        RuleFor(x => x.TimeSlot).NotEmpty();
        RuleFor(x => x.AppointmentDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date);
    }
}

public class BookServiceAppointmentCommandHandler : IRequestHandler<BookServiceAppointmentCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public BookServiceAppointmentCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(BookServiceAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appt = new ServiceAppointment(
            request.CompanyId,
            request.BranchId,
            request.BuyerId,
            request.VinNumber,
            request.RegistrationNumber,
            request.ServiceType,
            request.AppointmentDate,
            request.TimeSlot,
            request.EstimatedCost,
            request.CustomerComments,
            request.VehicleVariantId);

        _context.ServiceAppointments.Add(appt);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(appt.Id);
    }
}

// ── Update Service Appointment ──────────────────────────────
public record UpdateServiceAppointmentCommand(
    Guid Id,
    decimal ActualCost,
    string? WorkshopNotes = null,
    ServiceAppointmentStatus Status = ServiceAppointmentStatus.Completed) : IRequest<Result<bool>>;

public class UpdateServiceAppointmentCommandHandler : IRequestHandler<UpdateServiceAppointmentCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public UpdateServiceAppointmentCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateServiceAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appt = await _context.ServiceAppointments.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (appt == null) return Result<bool>.Failure("Service appointment not found.");

        if (request.Status == ServiceAppointmentStatus.Completed)
        {
            appt.CompleteService(request.ActualCost, request.WorkshopNotes);
        }
        else if (request.Status == ServiceAppointmentStatus.InProgress)
        {
            appt.StartService();
        }
        else if (request.Status == ServiceAppointmentStatus.Cancelled)
        {
            appt.CancelService();
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Cancel Appointment ──────────────────────────────────────
public record CancelServiceAppointmentCommand(Guid Id) : IRequest<Result<bool>>;

public class CancelServiceAppointmentCommandHandler : IRequestHandler<CancelServiceAppointmentCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public CancelServiceAppointmentCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(CancelServiceAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appt = await _context.ServiceAppointments.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (appt == null) return Result<bool>.Failure("Service appointment not found.");

        appt.CancelService();
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Queries ─────────────────────────────────────────────────
public record GetServiceAppointmentsQuery(Guid? BuyerId = null) : IRequest<Result<List<ServiceAppointmentDto>>>;

public class GetServiceAppointmentsQueryHandler : IRequestHandler<GetServiceAppointmentsQuery, Result<List<ServiceAppointmentDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetServiceAppointmentsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ServiceAppointmentDto>>> Handle(GetServiceAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ServiceAppointments.AsNoTracking();
        if (request.BuyerId.HasValue)
        {
            query = query.Where(s => s.BuyerId == request.BuyerId.Value);
        }

        var list = await query
            .OrderByDescending(s => s.AppointmentDate)
            .Select(s => new ServiceAppointmentDto(
                s.Id,
                s.CompanyId,
                s.BranchId,
                s.BuyerId,
                s.VinNumber,
                s.RegistrationNumber,
                s.VehicleVariantId,
                s.ServiceType,
                s.AppointmentDate,
                s.TimeSlot,
                s.CustomerComments,
                s.EstimatedCost,
                s.ActualCost,
                s.WorkshopNotes,
                s.Status,
                s.CompletedAt,
                s.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<ServiceAppointmentDto>>.Success(list);
    }
}

public record GetServiceHistoryByVinQuery(string Vin) : IRequest<Result<List<ServiceAppointmentDto>>>;

public class GetServiceHistoryByVinQueryHandler : IRequestHandler<GetServiceHistoryByVinQuery, Result<List<ServiceAppointmentDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetServiceHistoryByVinQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ServiceAppointmentDto>>> Handle(GetServiceHistoryByVinQuery request, CancellationToken cancellationToken)
    {
        var cleanVin = request.Vin.Trim().ToUpperInvariant();
        var history = await _context.ServiceAppointments.AsNoTracking()
            .Where(s => s.VinNumber == cleanVin)
            .OrderByDescending(s => s.AppointmentDate)
            .Select(s => new ServiceAppointmentDto(
                s.Id,
                s.CompanyId,
                s.BranchId,
                s.BuyerId,
                s.VinNumber,
                s.RegistrationNumber,
                s.VehicleVariantId,
                s.ServiceType,
                s.AppointmentDate,
                s.TimeSlot,
                s.CustomerComments,
                s.EstimatedCost,
                s.ActualCost,
                s.WorkshopNotes,
                s.Status,
                s.CompletedAt,
                s.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<ServiceAppointmentDto>>.Success(history);
    }
}

public record GetServiceAppointmentByIdQuery(Guid Id) : IRequest<Result<ServiceAppointmentDto>>;

public class GetServiceAppointmentByIdQueryHandler : IRequestHandler<GetServiceAppointmentByIdQuery, Result<ServiceAppointmentDto>>
{
    private readonly IIdentityDbContext _context;

    public GetServiceAppointmentByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ServiceAppointmentDto>> Handle(GetServiceAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var s = await _context.ServiceAppointments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (s == null) return Result<ServiceAppointmentDto>.Failure("Service appointment not found.");

        return Result<ServiceAppointmentDto>.Success(new ServiceAppointmentDto(
            s.Id,
            s.CompanyId,
            s.BranchId,
            s.BuyerId,
            s.VinNumber,
            s.RegistrationNumber,
            s.VehicleVariantId,
            s.ServiceType,
            s.AppointmentDate,
            s.TimeSlot,
            s.CustomerComments,
            s.EstimatedCost,
            s.ActualCost,
            s.WorkshopNotes,
            s.Status,
            s.CompletedAt,
            s.CreatedAt));
    }
}
