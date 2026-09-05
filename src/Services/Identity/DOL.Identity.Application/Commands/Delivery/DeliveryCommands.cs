using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Delivery;

public record SubmitPdiCommand(
    Guid CompanyId,
    Guid BranchId,
    Guid OrderId,
    Guid VehicleStockId,
    Guid InspectorStaffId,
    int OdometerReadingKm,
    int BatteryHealthPct,
    bool ExteriorConditionOk,
    bool InteriorCleanOk,
    bool ToolKitAndSpareWheelOk,
    bool DocumentationOk,
    string? InspectionNotes = null) : IRequest<Result<Guid>>;

public class SubmitPdiCommandValidator : AbstractValidator<SubmitPdiCommand>
{
    public SubmitPdiCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.VehicleStockId).NotEmpty();
        RuleFor(x => x.OdometerReadingKm).GreaterThanOrEqualTo(0);
        RuleFor(x => x.BatteryHealthPct).InclusiveBetween(1, 100);
    }
}

public class SubmitPdiCommandHandler : IRequestHandler<SubmitPdiCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public SubmitPdiCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(SubmitPdiCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.VehicleOrders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        if (order == null) return Result<Guid>.Failure("Order not found.");

        var pdi = new DeliveryInspection(
            request.CompanyId,
            request.BranchId,
            request.OrderId,
            request.VehicleStockId,
            request.InspectorStaffId,
            request.OdometerReadingKm,
            request.BatteryHealthPct,
            request.ExteriorConditionOk,
            request.InteriorCleanOk,
            request.ToolKitAndSpareWheelOk,
            request.DocumentationOk,
            request.InspectionNotes);

        _context.DeliveryInspections.Add(pdi);

        // Advance order state to PDI Ready
        order.AdvanceStatus(OrderStatus.PdiReady);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(pdi.Id);
    }
}

public record GetPdiReportQuery(Guid OrderId) : IRequest<Result<DeliveryInspectionDto>>;

public class GetPdiReportQueryHandler : IRequestHandler<GetPdiReportQuery, Result<DeliveryInspectionDto>>
{
    private readonly IIdentityDbContext _context;

    public GetPdiReportQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DeliveryInspectionDto>> Handle(GetPdiReportQuery request, CancellationToken cancellationToken)
    {
        var pdi = await _context.DeliveryInspections
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.OrderId == request.OrderId, cancellationToken);

        if (pdi == null) return Result<DeliveryInspectionDto>.Failure("PDI inspection report not found for this order.");

        var dto = new DeliveryInspectionDto(
            pdi.Id,
            pdi.CompanyId,
            pdi.BranchId,
            pdi.OrderId,
            pdi.VehicleStockId,
            pdi.InspectorStaffId,
            pdi.OdometerReadingKm,
            pdi.BatteryHealthPct,
            pdi.ExteriorConditionOk,
            pdi.InteriorCleanOk,
            pdi.ToolKitAndSpareWheelOk,
            pdi.DocumentationOk,
            pdi.InspectionNotes,
            pdi.IsCustomerAccepted,
            pdi.CustomerSignatureUrl,
            pdi.CreatedAt);

        return Result<DeliveryInspectionDto>.Success(dto);
    }
}

public record GenerateDeliveryOtpCommand(Guid OrderId) : IRequest<Result<string>>;

public class GenerateDeliveryOtpCommandHandler : IRequestHandler<GenerateDeliveryOtpCommand, Result<string>>
{
    private readonly IIdentityDbContext _context;

    public GenerateDeliveryOtpCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<string>> Handle(GenerateDeliveryOtpCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.VehicleOrders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        if (order == null) return Result<string>.Failure("Order not found.");

        order.RegenerateOtp();
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success($"Delivery OTP generated successfully: {order.DeliveryOtp} (valid for order {order.OrderNumber})");
    }
}
