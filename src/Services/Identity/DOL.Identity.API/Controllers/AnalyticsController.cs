using System.Security.Claims;
using DOL.Identity.Application.Commands.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize(Roles = "Admin,GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager,SalesExecutive")]
public class AnalyticsController : ApiControllerBase
{
    private Guid ResolveCompanyId(Guid companyId)
    {
        if (companyId != Guid.Empty) return companyId;
        var tenantClaim = User.FindFirst("tenant_id")?.Value ?? User.FindFirst("companyId")?.Value;
        if (Guid.TryParse(tenantClaim, out var tenantId)) return tenantId;
        return Guid.Empty;
    }

    /// <summary>
    /// Dealership sales conversion funnel: Leads -> Quotations -> Bookings -> Loans -> Deliveries.
    /// </summary>
    [HttpGet("sales-funnel")]
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetSalesFunnel([FromQuery] Guid companyId, [FromQuery] Guid? branchId)
    {
        var resolved = ResolveCompanyId(companyId);
        var result = await Mediator.Send(new GetSalesFunnelQuery(resolved, branchId));
        return HandleResult(result);
    }

    /// <summary>
    /// Yard vehicle inventory aging BI analysis: <30 days, 31-60 days, 61-90 days, >90 days.
    /// </summary>
    [HttpGet("stock-aging")]
    [HttpGet("inventory-aging")]
    public async Task<IActionResult> GetStockAging([FromQuery] Guid companyId, [FromQuery] Guid? branchId)
    {
        var resolved = ResolveCompanyId(companyId);
        var result = await Mediator.Send(new GetStockAgingQuery(resolved, branchId));
        return HandleResult(result);
    }

    /// <summary>
    /// Financial and gross revenue metrics: Booking advances, Down payments, Loan disbursements, Workshop revenue.
    /// </summary>
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueAnalytics([FromQuery] Guid companyId, [FromQuery] Guid? branchId)
    {
        var resolved = ResolveCompanyId(companyId);
        var result = await Mediator.Send(new GetRevenueAnalyticsQuery(resolved, branchId));
        return HandleResult(result);
    }
}
