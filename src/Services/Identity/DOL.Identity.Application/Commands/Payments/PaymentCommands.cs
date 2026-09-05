using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Payments;

// ── Initiate Payment ────────────────────────────────────────
public record InitiatePaymentCommand(
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    decimal Amount,
    PaymentPurpose Purpose,
    string GatewayProvider = "Razorpay",
    string PaymentMode = "UPI",
    Guid? OrderId = null,
    Guid? QuotationId = null) : IRequest<Result<PaymentTransactionDto>>;

public class InitiatePaymentCommandValidator : AbstractValidator<InitiatePaymentCommand>
{
    public InitiatePaymentCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.GatewayProvider).NotEmpty();
    }
}

public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, Result<PaymentTransactionDto>>
{
    private readonly IIdentityDbContext _context;

    public InitiatePaymentCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaymentTransactionDto>> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
    {
        string refNumber = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        string mockGatewayOrderId = $"order_{Guid.NewGuid().ToString("N")[..12]}";

        var txn = new PaymentTransaction(
            request.CompanyId,
            request.BranchId,
            request.BuyerId,
            refNumber,
            request.Amount,
            request.Purpose,
            request.GatewayProvider,
            request.PaymentMode,
            request.OrderId,
            request.QuotationId,
            mockGatewayOrderId);

        _context.PaymentTransactions.Add(txn);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new PaymentTransactionDto(
            txn.Id,
            txn.CompanyId,
            txn.BranchId,
            txn.BuyerId,
            txn.OrderId,
            txn.QuotationId,
            txn.TransactionReference,
            txn.GatewayProvider,
            txn.GatewayPaymentId,
            txn.GatewayOrderId,
            txn.Amount,
            txn.Currency,
            txn.Purpose,
            txn.Status,
            txn.PaymentMode,
            txn.PaidAt,
            txn.FailureReason,
            txn.ReceiptUrl,
            txn.CreatedAt);

        return Result<PaymentTransactionDto>.Success(dto);
    }
}

// ── Verify Payment ──────────────────────────────────────────
public record VerifyPaymentCommand(
    string TransactionReference,
    string GatewayPaymentId,
    string? Signature = null,
    string? ReceiptUrl = null) : IRequest<Result<bool>>;

public class VerifyPaymentCommandHandler : IRequestHandler<VerifyPaymentCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public VerifyPaymentCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
    {
        var txn = await _context.PaymentTransactions
            .FirstOrDefaultAsync(t => t.TransactionReference == request.TransactionReference.Trim(), cancellationToken);

        if (txn == null) return Result<bool>.Failure("Payment transaction reference not found.");

        txn.MarkSuccessful(request.GatewayPaymentId, request.ReceiptUrl);

        // If payment is linked to a VehicleOrder, update paid amounts
        if (txn.OrderId.HasValue)
        {
            var order = await _context.VehicleOrders.FirstOrDefaultAsync(o => o.Id == txn.OrderId.Value, cancellationToken);
            if (order != null)
            {
                if (txn.Purpose == PaymentPurpose.DownPayment)
                {
                    order.RecordDownPayment(txn.Amount);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Refund Payment ──────────────────────────────────────────
public record RefundPaymentCommand(Guid Id, string? Reason = null) : IRequest<Result<bool>>;

public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public RefundPaymentCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var txn = await _context.PaymentTransactions.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (txn == null) return Result<bool>.Failure("Transaction not found.");

        if (txn.Status != PaymentStatus.Successful)
        {
            return Result<bool>.Failure("Only successful transactions can be refunded.");
        }

        txn.ProcessRefund(request.Reason ?? "Customer cancelled vehicle booking / order.");
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Queries ─────────────────────────────────────────────────
public record GetPaymentsQuery(Guid? BuyerId = null, Guid? OrderId = null) : IRequest<Result<List<PaymentTransactionDto>>>;

public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, Result<List<PaymentTransactionDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetPaymentsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<PaymentTransactionDto>>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PaymentTransactions.AsNoTracking();

        if (request.BuyerId.HasValue)
        {
            query = query.Where(t => t.BuyerId == request.BuyerId.Value);
        }

        if (request.OrderId.HasValue)
        {
            query = query.Where(t => t.OrderId == request.OrderId.Value);
        }

        var list = await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new PaymentTransactionDto(
                t.Id,
                t.CompanyId,
                t.BranchId,
                t.BuyerId,
                t.OrderId,
                t.QuotationId,
                t.TransactionReference,
                t.GatewayProvider,
                t.GatewayPaymentId,
                t.GatewayOrderId,
                t.Amount,
                t.Currency,
                t.Purpose,
                t.Status,
                t.PaymentMode,
                t.PaidAt,
                t.FailureReason,
                t.ReceiptUrl,
                t.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<PaymentTransactionDto>>.Success(list);
    }
}

public record GetPaymentByIdQuery(Guid Id) : IRequest<Result<PaymentTransactionDto>>;

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, Result<PaymentTransactionDto>>
{
    private readonly IIdentityDbContext _context;

    public GetPaymentByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaymentTransactionDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var t = await _context.PaymentTransactions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (t == null) return Result<PaymentTransactionDto>.Failure("Transaction not found.");

        return Result<PaymentTransactionDto>.Success(new PaymentTransactionDto(
            t.Id,
            t.CompanyId,
            t.BranchId,
            t.BuyerId,
            t.OrderId,
            t.QuotationId,
            t.TransactionReference,
            t.GatewayProvider,
            t.GatewayPaymentId,
            t.GatewayOrderId,
            t.Amount,
            t.Currency,
            t.Purpose,
            t.Status,
            t.PaymentMode,
            t.PaidAt,
            t.FailureReason,
            t.ReceiptUrl,
            t.CreatedAt));
    }
}
