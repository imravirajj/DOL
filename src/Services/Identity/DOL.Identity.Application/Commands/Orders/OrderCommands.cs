using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Orders;

// ── Convert Quotation to Order ──────────────────────────────
public record CreateOrderFromQuotationCommand(
    Guid QuotationId,
    decimal BookingAmountPaid,
    DeliveryType DeliveryType,
    Guid? AllocatedStockId = null) : IRequest<Result<Guid>>;

public class CreateOrderFromQuotationCommandHandler : IRequestHandler<CreateOrderFromQuotationCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public CreateOrderFromQuotationCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateOrderFromQuotationCommand request, CancellationToken cancellationToken)
    {
        var quotation = await _context.Quotations.FirstOrDefaultAsync(q => q.Id == request.QuotationId, cancellationToken);
        if (quotation == null) return Result<Guid>.Failure("Quotation not found.");

        if (quotation.Status == QuotationStatus.ConvertedToBooking)
        {
            return Result<Guid>.Failure("Quotation has already been converted to a booking/order.");
        }

        var orderCount = await _context.VehicleOrders.CountAsync(cancellationToken);
        var orderNumber = $"ORD-2026-{orderCount + 1001:D5}";

        var order = new VehicleOrder(
            quotation.CompanyId,
            quotation.BranchId,
            quotation.BuyerId ?? Guid.NewGuid(),
            quotation.Id,
            quotation.VehicleVariantId,
            orderNumber,
            quotation.TotalOnRoadPrice,
            request.BookingAmountPaid,
            request.DeliveryType,
            request.AllocatedStockId);

        quotation.MarkConvertedToBooking();
        _context.VehicleOrders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(order.Id);
    }
}

// ── Advance Order Status ────────────────────────────────────
public record UpdateOrderStatusCommand(
    Guid Id,
    OrderStatus Status,
    Guid? AllocatedStockId = null) : IRequest<Result>;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateOrderStatusCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.VehicleOrders.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        if (order == null) return Result.Failure("Order not found.");

        if (request.AllocatedStockId.HasValue)
        {
            order.AllocateStock(request.AllocatedStockId.Value);
        }

        order.AdvanceStatus(request.Status);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

// ── Verify Delivery OTP ─────────────────────────────────────
public record VerifyDeliveryOtpCommand(
    Guid Id,
    string Otp) : IRequest<Result<string>>;

public class VerifyDeliveryOtpCommandHandler : IRequestHandler<VerifyDeliveryOtpCommand, Result<string>>
{
    private readonly IIdentityDbContext _context;

    public VerifyDeliveryOtpCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<string>> Handle(VerifyDeliveryOtpCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.VehicleOrders
            .Include(o => o.AllocatedStock)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null) return Result<string>.Failure("Order not found.");

        bool isDelivered = order.VerifyAndDeliver(request.Otp);
        if (!isDelivered)
        {
            return Result<string>.Failure("Invalid Delivery OTP or vehicle is not ready for delivery handover.");
        }

        if (order.AllocatedStock != null)
        {
            order.AllocatedStock.SetStatus(VehicleStockStatus.Delivered);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<string>.Success($"Vehicle order {order.OrderNumber} successfully marked as Delivered! Key handover verified.");
    }
}

// ── Delete / Cancel Order ───────────────────────────────────
public record DeleteOrderCommand(Guid Id) : IRequest<Result>;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public DeleteOrderCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.VehicleOrders.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        if (order == null) return Result.Failure("Order not found.");

        order.AdvanceStatus(OrderStatus.Cancelled);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

// ── Queries ─────────────────────────────────────────────────
public record GetOrdersQuery(
    Guid? BuyerId = null,
    OrderStatus? Status = null) : IRequest<Result<List<VehicleOrderDto>>>;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, Result<List<VehicleOrderDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetOrdersQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<VehicleOrderDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VehicleOrders
            .Include(o => o.AllocatedStock)
            .AsNoTracking();

        if (request.BuyerId.HasValue) query = query.Where(o => o.BuyerId == request.BuyerId.Value);
        if (request.Status.HasValue) query = query.Where(o => o.Status == request.Status.Value);

        var list = await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new VehicleOrderDto(
                o.Id,
                o.CompanyId,
                o.BranchId,
                o.BuyerId,
                o.QuotationId,
                o.VehicleVariantId,
                o.AllocatedStockId,
                o.AllocatedStock != null ? o.AllocatedStock.VinNumber : null,
                o.OrderNumber,
                o.TotalAmount,
                o.BookingAmountPaid,
                o.DownPaymentPaid,
                o.LoanDisbursedAmount,
                o.Status,
                o.DeliveryType,
                o.DeliveryOtp,
                o.DeliveredAt,
                o.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<VehicleOrderDto>>.Success(list);
    }
}

public record GetOrderByIdQuery(Guid Id) : IRequest<Result<VehicleOrderDto>>;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<VehicleOrderDto>>
{
    private readonly IIdentityDbContext _context;

    public GetOrderByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<VehicleOrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var o = await _context.VehicleOrders
            .Include(x => x.AllocatedStock)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (o == null) return Result<VehicleOrderDto>.Failure("Order not found.");

        var dto = new VehicleOrderDto(
            o.Id,
            o.CompanyId,
            o.BranchId,
            o.BuyerId,
            o.QuotationId,
            o.VehicleVariantId,
            o.AllocatedStockId,
            o.AllocatedStock != null ? o.AllocatedStock.VinNumber : null,
            o.OrderNumber,
            o.TotalAmount,
            o.BookingAmountPaid,
            o.DownPaymentPaid,
            o.LoanDisbursedAmount,
            o.Status,
            o.DeliveryType,
            o.DeliveryOtp,
            o.DeliveredAt,
            o.CreatedAt);

        return Result<VehicleOrderDto>.Success(dto);
    }
}
