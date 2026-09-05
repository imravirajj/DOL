using DOL.Identity.Application.Commands.Locations;
using DOL.Identity.Application.Queries.Locations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class LocationController : ApiControllerBase
{
    // ── Countries ─────────────────────────────────────────────
    [HttpGet("countries")]
    public async Task<IActionResult> GetCountries()
    {
        var result = await Mediator.Send(new GetLocationsQuery());
        return HandleResult(result);
    }

    [HttpPost("countries")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> CreateCountry([FromBody] CreateCountryCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("countries/{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> UpdateCountry(Guid id, [FromBody] UpdateCountryCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("countries/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCountry(Guid id)
    {
        var result = await Mediator.Send(new DeleteCountryCommand(id));
        return HandleResult(result);
    }

    // ── States / Regions ──────────────────────────────────────
    [HttpGet("states")]
    public async Task<IActionResult> GetStates([FromQuery] Guid? countryId)
    {
        var result = await Mediator.Send(new GetStatesQuery(countryId));
        return HandleResult(result);
    }

    [HttpPost("states")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> CreateState([FromBody] CreateStateRegionCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("states/{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> UpdateState(Guid id, [FromBody] UpdateStateCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("states/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteState(Guid id)
    {
        var result = await Mediator.Send(new DeleteStateCommand(id));
        return HandleResult(result);
    }

    // ── Cities ────────────────────────────────────────────────
    [HttpGet("cities")]
    public async Task<IActionResult> GetCities([FromQuery] Guid? stateId)
    {
        var result = await Mediator.Send(new GetCitiesQuery(stateId));
        return HandleResult(result);
    }

    [HttpPost("cities")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> CreateCity([FromBody] CreateCityCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("cities/{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> UpdateCity(Guid id, [FromBody] UpdateCityCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("cities/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCity(Guid id)
    {
        var result = await Mediator.Send(new DeleteCityCommand(id));
        return HandleResult(result);
    }
}
