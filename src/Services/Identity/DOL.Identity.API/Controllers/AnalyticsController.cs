using DOL.Identity.Application.Commands.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager")]
public class AnalyticsController : ApiControllerBase
{
    /// <summary>
    /// Dealership sales conversion funnel: Leads -> Quotations -> Bookings -> Loans -> Deliveries.
    /// </summary>
    [HttpGet("sales-funnel")]
    public async Task<IActionResult> GetSalesFunnel([FromQuery] Guid companyId, [FromQuery] Guid? branchId)
    {
        var result = await Mediator.Send(new GetSalesFunnelQuery(companyId, branchId));
        return HandleResult(result);
    }

    /// <summary>
    /// Yard vehicle inventory aging BI analysis: <30 days, 31-60 days, 61-90 days, >90 days.
    /// </summary>
    [HttpGet("stock-aging")]
    public async Task<IActionResult> GetStockAging([FromQuery] Guid companyId, [FromQuery] Guid? branchId)
    {
        var result = await Mediator.Send(new GetStockAgingQuery(companyId, branchId));
        return HandleResult(result);
    }

    /// <summary>
    /// Financial and gross revenue metrics: Booking advances, Down payments, Loan disbursements, Workshop revenue.
    /// </summary>
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueAnalytics([FromQuery] Guid companyId, [FromQuery] Guid? branchId)
    {
        var result = await Mediator.Send(new GetRevenueAnalyticsQuery(companyId, branchId));
        return HandleResult(result);
    }
}
