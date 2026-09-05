using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.TestDrives;

public record BookTestDriveCommand(
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    Guid VehicleVariantId,
    string CustomerName,
    string CustomerPhone,
    string CustomerEmail,
    string DrivingLicenseNumber,
    DateTime ScheduledDate,
    string TimeSlot,
    DeliveryType LocationType = DeliveryType.ShowroomPickup,
    string? HomeAddress = null) : IRequest<Result<Guid>>;

public class BookTestDriveCommandValidator : AbstractValidator<BookTestDriveCommand>
{
    public BookTestDriveCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.VehicleVariantId).NotEmpty();
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CustomerPhone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.CustomerEmail).EmailAddress();
        RuleFor(x => x.DrivingLicenseNumber).NotEmpty().MaximumLength(30);
    }
}

public class BookTestDriveCommandHandler : IRequestHandler<BookTestDriveCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public BookTestDriveCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(BookTestDriveCommand request, CancellationToken cancellationToken)
    {
        var booking = new TestDriveBooking(
            request.CompanyId,
            request.BranchId,
            request.BuyerId,
            request.VehicleVariantId,
            request.CustomerName,
            request.CustomerPhone,
            request.CustomerEmail,
            request.DrivingLicenseNumber,
            request.ScheduledDate,
            request.TimeSlot,
            request.LocationType,
            request.HomeAddress);

        _context.TestDriveBookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(booking.Id);
    }
}

public record UpdateTestDriveCommand(
    Guid Id,
    DateTime? RescheduledDate = null,
    string? NewTimeSlot = null,
    TestDriveStatus? Status = null,
    int? Rating = null,
    string? FeedbackNotes = null) : IRequest<Result>;

public class UpdateTestDriveCommandHandler : IRequestHandler<UpdateTestDriveCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateTestDriveCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateTestDriveCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.TestDriveBookings.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (booking == null) return Result.Failure("Test drive booking not found.");

        if (request.RescheduledDate.HasValue && !string.IsNullOrEmpty(request.NewTimeSlot))
        {
            booking.Reschedule(request.RescheduledDate.Value, request.NewTimeSlot);
        }

        if (request.Rating.HasValue)
        {
            booking.Complete(request.Rating.Value, request.FeedbackNotes);
        }
        else if (request.Status.HasValue)
        {
            booking.SetStatus(request.Status.Value);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteTestDriveCommand(Guid Id) : IRequest<Result>;

public class DeleteTestDriveCommandHandler : IRequestHandler<DeleteTestDriveCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public DeleteTestDriveCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteTestDriveCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.TestDriveBookings.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (booking == null) return Result.Failure("Test drive booking not found.");

        booking.SetStatus(TestDriveStatus.Cancelled);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public record GetTestDrivesQuery(
    Guid? BuyerId = null,
    TestDriveStatus? Status = null) : IRequest<Result<List<TestDriveBookingDto>>>;

public class GetTestDrivesQueryHandler : IRequestHandler<GetTestDrivesQuery, Result<List<TestDriveBookingDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetTestDrivesQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TestDriveBookingDto>>> Handle(GetTestDrivesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TestDriveBookings
            .Include(t => t.VehicleVariant)
            .AsNoTracking();

        if (request.BuyerId.HasValue) query = query.Where(t => t.BuyerId == request.BuyerId.Value);
        if (request.Status.HasValue) query = query.Where(t => t.Status == request.Status.Value);

        var list = await query
            .OrderByDescending(t => t.ScheduledDate)
            .Select(t => new TestDriveBookingDto(
                t.Id,
                t.CompanyId,
                t.BranchId,
                t.BuyerId,
                t.VehicleVariantId,
                t.VehicleVariant != null ? t.VehicleVariant.VariantName : null,
                t.CustomerName,
                t.CustomerPhone,
                t.CustomerEmail,
                t.DrivingLicenseNumber,
                t.ScheduledDate,
                t.TimeSlot,
                t.LocationType,
                t.HomeAddress,
                t.Status,
                t.Rating,
                t.FeedbackNotes,
                t.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<TestDriveBookingDto>>.Success(list);
    }
}

public record GetTestDriveByIdQuery(Guid Id) : IRequest<Result<TestDriveBookingDto>>;

public class GetTestDriveByIdQueryHandler : IRequestHandler<GetTestDriveByIdQuery, Result<TestDriveBookingDto>>
{
    private readonly IIdentityDbContext _context;

    public GetTestDriveByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TestDriveBookingDto>> Handle(GetTestDriveByIdQuery request, CancellationToken cancellationToken)
    {
        var t = await _context.TestDriveBookings
            .Include(x => x.VehicleVariant)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (t == null) return Result<TestDriveBookingDto>.Failure("Test drive booking not found.");

        var dto = new TestDriveBookingDto(
            t.Id,
            t.CompanyId,
            t.BranchId,
            t.BuyerId,
            t.VehicleVariantId,
            t.VehicleVariant != null ? t.VehicleVariant.VariantName : null,
            t.CustomerName,
            t.CustomerPhone,
            t.CustomerEmail,
            t.DrivingLicenseNumber,
            t.ScheduledDate,
            t.TimeSlot,
            t.LocationType,
            t.HomeAddress,
            t.Status,
            t.Rating,
            t.FeedbackNotes,
            t.CreatedAt);

        return Result<TestDriveBookingDto>.Success(dto);
    }
}
