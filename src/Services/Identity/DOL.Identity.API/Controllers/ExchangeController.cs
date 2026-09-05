using DOL.Identity.Application.Commands.Exchange;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class ExchangeController : ApiControllerBase
{
    /// <summary>
    /// Evaluates old car valuation based on age, mileage, fuel type, and condition.
    /// </summary>
    [HttpPost("valuate")]
    public async Task<IActionResult> ValuateTradeIn([FromBody] ValuateTradeInCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Schedules physical inspection appointment for old car.
    /// </summary>
    [HttpPost("{id:guid}/schedule-inspection")]
    public async Task<IActionResult> ScheduleInspection(Guid id, [FromBody] DateTime inspectionDate)
    {
        var result = await Mediator.Send(new ScheduleInspectionCommand(id, inspectionDate));
        return HandleResult(result);
    }

    /// <summary>
    /// Dealer/Evaluator provides final trade-in offer price and inspection remarks.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager,SalesExecutive")]
    [HttpPut("{id:guid}/offer")]
    public async Task<IActionResult> ProvideOffer(Guid id, [FromBody] ProvideOfferCommand command)
    {
        if (id != command.TradeInId) return BadRequest(new { errors = new[] { "Route ID does not match body TradeInId." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Customer accepts or rejects the trade-in valuation offer.
    /// </summary>
    [HttpPost("{id:guid}/respond-offer")]
    public async Task<IActionResult> RespondToOffer(Guid id, [FromQuery] bool accept)
    {
        var result = await Mediator.Send(new RespondToOfferCommand(id, accept));
        return HandleResult(result);
    }

    /// <summary>
    /// Lists old car exchange requests, optionally filtered by buyer ID.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTradeIns([FromQuery] Guid? buyerId)
    {
        var result = await Mediator.Send(new GetTradeInsQuery(buyerId));
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves a single trade-in exchange case by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTradeInById(Guid id)
    {
        var result = await Mediator.Send(new GetTradeInByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Cancels / deletes a trade-in exchange request.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTradeIn(Guid id)
    {
        var result = await Mediator.Send(new DeleteTradeInCommand(id));
        return HandleResult(result);
    }
}
