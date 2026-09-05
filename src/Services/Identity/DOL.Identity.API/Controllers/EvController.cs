using DOL.Identity.Application.Commands.Ev;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class EvController : ApiControllerBase
{
    /// <summary>
    /// Searches nearby EV fast-charging stations, optionally filtered by connector type (e.g. CCS2, Type2).
    /// </summary>
    [AllowAnonymous]
    [HttpGet("stations")]
    public async Task<IActionResult> GetChargingStations([FromQuery] string? connectorType)
    {
        var result = await Mediator.Send(new GetChargingStationsQuery(connectorType));
        return HandleResult(result);
    }

    /// <summary>
    /// Registers a new dealership fast charging station or public charging hub.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager")]
    [HttpPost("stations")]
    public async Task<IActionResult> CreateChargingStation([FromBody] CreateChargingStationCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Books a home charger site survey and installation request for an EV buyer.
    /// </summary>
    [HttpPost("home-charger/request")]
    public async Task<IActionResult> RequestHomeCharger([FromBody] RequestHomeChargerCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Tracks home charger installation and survey status by order ID.
    /// </summary>
    [HttpGet("home-charger/{orderId:guid}")]
    public async Task<IActionResult> GetHomeChargerByOrder(Guid orderId)
    {
        var result = await Mediator.Send(new GetHomeChargerByOrderQuery(orderId));
        return HandleResult(result);
    }

    /// <summary>
    /// Field technician updates home charger survey and installation progress.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager,SalesExecutive")]
    [HttpPut("home-charger/{id:guid}/status")]
    public async Task<IActionResult> UpdateHomeChargerStatus(Guid id, [FromBody] UpdateHomeChargerStatusCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
