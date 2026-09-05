using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Analytics;

// ── Sales Funnel BI Query ───────────────────────────────────
public record GetSalesFunnelQuery(Guid CompanyId, Guid? BranchId = null) : IRequest<Result<SalesFunnelDto>>;

public class GetSalesFunnelQueryHandler : IRequestHandler<GetSalesFunnelQuery, Result<SalesFunnelDto>>
{
    private readonly IIdentityDbContext _context;

    public GetSalesFunnelQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SalesFunnelDto>> Handle(GetSalesFunnelQuery request, CancellationToken cancellationToken)
    {
        var quotationsQuery = _context.Quotations.AsNoTracking().Where(q => q.CompanyId == request.CompanyId);
        var ordersQuery = _context.VehicleOrders.AsNoTracking().Where(o => o.CompanyId == request.CompanyId);
        var loansQuery = _context.LoanApplications.AsNoTracking().Where(l => l.CompanyId == request.CompanyId);

        if (request.BranchId.HasValue)
        {
            quotationsQuery = quotationsQuery.Where(q => q.BranchId == request.BranchId.Value);
            ordersQuery = ordersQuery.Where(o => o.BranchId == request.BranchId.Value);
            loansQuery = loansQuery.Where(l => l.BranchId == request.BranchId.Value);
        }

        int totalQuotations = await quotationsQuery.CountAsync(cancellationToken);
        int totalOrders = await ordersQuery.CountAsync(cancellationToken);
        int pendingLoans = await loansQuery.CountAsync(l => l.Status == LoanStatus.Applied || l.Status == LoanStatus.UnderReview, cancellationToken);
        int approvedLoans = await loansQuery.CountAsync(l => l.Status == LoanStatus.Sanctioned || l.Status == LoanStatus.Disbursed, cancellationToken);
        int completedDeliveries = await ordersQuery.CountAsync(o => o.Status == OrderStatus.Delivered, cancellationToken);

        decimal leadToOrderConversion = totalQuotations > 0 ? Math.Round(((decimal)totalOrders / totalQuotations) * 100m, 2) : 0m;
        decimal orderToDeliveryConversion = totalOrders > 0 ? Math.Round(((decimal)completedDeliveries / totalOrders) * 100m, 2) : 0m;

        var funnel = new SalesFunnelDto(
            totalQuotations,
            totalOrders,
            pendingLoans,
            approvedLoans,
            completedDeliveries,
            leadToOrderConversion,
            orderToDeliveryConversion);

        return Result<SalesFunnelDto>.Success(funnel);
    }
}

// ── Stock Aging Yard BI Query ───────────────────────────────
public record GetStockAgingQuery(Guid CompanyId, Guid? BranchId = null) : IRequest<Result<StockAgingDto>>;

public class GetStockAgingQueryHandler : IRequestHandler<GetStockAgingQuery, Result<StockAgingDto>>
{
    private readonly IIdentityDbContext _context;

    public GetStockAgingQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StockAgingDto>> Handle(GetStockAgingQuery request, CancellationToken cancellationToken)
    {
        var stocksQuery = _context.VehicleStocks.AsNoTracking()
            .Include(s => s.VehicleVariant)
            .Where(s => s.CompanyId == request.CompanyId && s.Status == VehicleStockStatus.Available);

        if (request.BranchId.HasValue)
        {
            stocksQuery = stocksQuery.Where(s => s.BranchId == request.BranchId.Value);
        }

        var stocks = await stocksQuery.ToListAsync(cancellationToken);
        var now = DateTime.UtcNow;

        int under30 = 0;
        int between31And60 = 0;
        int between61And90 = 0;
        int over90 = 0;
        decimal totalValue = 0;

        foreach (var s in stocks)
        {
            var days = (now - s.CreatedAt).TotalDays;
            totalValue += s.VehicleVariant?.ExShowroomPrice ?? 0m;

            if (days <= 30) under30++;
            else if (days <= 60) between31And60++;
            else if (days <= 90) between61And90++;
            else over90++;
        }

        var aging = new StockAgingDto(
            stocks.Count,
            under30,
            between31And60,
            between61And90,
            over90,
            totalValue);

        return Result<StockAgingDto>.Success(aging);
    }
}

// ── Revenue Analytics BI Query ──────────────────────────────
public record GetRevenueAnalyticsQuery(Guid CompanyId, Guid? BranchId = null) : IRequest<Result<RevenueAnalyticsDto>>;

public class GetRevenueAnalyticsQueryHandler : IRequestHandler<GetRevenueAnalyticsQuery, Result<RevenueAnalyticsDto>>
{
    private readonly IIdentityDbContext _context;

    public GetRevenueAnalyticsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RevenueAnalyticsDto>> Handle(GetRevenueAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var ordersQuery = _context.VehicleOrders.AsNoTracking().Where(o => o.CompanyId == request.CompanyId);
        var serviceQuery = _context.ServiceAppointments.AsNoTracking().Where(s => s.CompanyId == request.CompanyId && s.Status == ServiceAppointmentStatus.Completed);
        var accessoriesQuery = _context.VehicleAccessories.AsNoTracking().Where(a => a.CompanyId == request.CompanyId);

        if (request.BranchId.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.BranchId == request.BranchId.Value);
            serviceQuery = serviceQuery.Where(s => s.BranchId == request.BranchId.Value);
        }

        decimal totalOrderValue = await ordersQuery.SumAsync(o => (decimal?)o.TotalAmount, cancellationToken) ?? 0m;
        decimal totalBookingPaid = await ordersQuery.SumAsync(o => (decimal?)o.BookingAmountPaid, cancellationToken) ?? 0m;
        decimal totalDownPayment = await ordersQuery.SumAsync(o => (decimal?)o.DownPaymentPaid, cancellationToken) ?? 0m;
        decimal totalLoanDisbursed = await ordersQuery.SumAsync(o => (decimal?)o.LoanDisbursedAmount, cancellationToken) ?? 0m;
        decimal totalServiceRevenue = await serviceQuery.SumAsync(s => s.ActualCost, cancellationToken) ?? 0m;
        decimal totalAccessoriesInventoryValue = await accessoriesQuery.SumAsync(a => (decimal?)a.Price, cancellationToken) ?? 0m;

        var rev = new RevenueAnalyticsDto(
            totalOrderValue,
            totalBookingPaid,
            totalDownPayment,
            totalLoanDisbursed,
            totalAccessoriesInventoryValue,
            totalServiceRevenue);

        return Result<RevenueAnalyticsDto>.Success(rev);
    }
}
