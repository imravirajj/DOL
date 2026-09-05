using DOL.Identity.Application.Commands.Vehicles;
using DOL.Identity.Application.Queries.Vehicles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

public class VehicleCatalogController : ApiControllerBase
{
    // ── Vehicle Models ──────────────────────────────────────────
    [HttpGet("models")]
    [AllowAnonymous]
    public async Task<IActionResult> GetModels([FromQuery] string? category, [FromQuery] bool? activeOnly = true)
    {
        var result = await Mediator.Send(new GetVehicleModelsQuery(category, activeOnly));
        return HandleResult(result);
    }

    [HttpGet("models/{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetModelById(Guid id)
    {
        var result = await Mediator.Send(new GetVehicleModelByIdQuery(id));
        return HandleResult(result);
    }

    [HttpPost("models")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> CreateModel([FromBody] CreateVehicleModelCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("models/{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> UpdateModel(Guid id, [FromBody] UpdateVehicleModelCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("models/{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> DeleteModel(Guid id)
    {
        var result = await Mediator.Send(new DeleteVehicleModelCommand(id));
        return HandleResult(result);
    }

    // ── Vehicle Variants ────────────────────────────────────────
    [HttpGet("variants")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVariants([FromQuery] Guid? modelId, [FromQuery] bool? activeOnly = true)
    {
        var result = await Mediator.Send(new GetVehicleVariantsQuery(modelId, activeOnly));
        return HandleResult(result);
    }

    [HttpGet("variants/{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVariantById(Guid id)
    {
        var result = await Mediator.Send(new GetVehicleVariantByIdQuery(id));
        return HandleResult(result);
    }

    [HttpPost("variants")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> CreateVariant([FromBody] CreateVehicleVariantCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("variants/{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> UpdateVariant(Guid id, [FromBody] UpdateVehicleVariantCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("variants/{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> DeleteVariant(Guid id)
    {
        var result = await Mediator.Send(new DeleteVehicleVariantCommand(id));
        return HandleResult(result);
    }
}
