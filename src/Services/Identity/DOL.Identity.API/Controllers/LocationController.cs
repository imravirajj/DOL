using DOL.Identity.Application.Commands.Locations;
using DOL.Identity.Application.Queries.Locations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Route("api/[controller]")]
[Route("api/locations")]
[Authorize]
public class LocationController : ApiControllerBase
{
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

    [HttpPost("states")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> CreateState([FromBody] CreateStateRegionCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("cities")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> CreateCity([FromBody] CreateCityCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
