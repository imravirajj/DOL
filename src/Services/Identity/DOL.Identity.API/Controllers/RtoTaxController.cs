using DOL.Identity.Application.Commands.Rto;
using DOL.Identity.Application.Queries.Rto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class RtoTaxController : ApiControllerBase
{
    /// <summary>
    /// Lists all state-wise RTO tax slabs, optionally filtered by state name.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetRtoTaxSlabs([FromQuery] string? stateName)
    {
        var result = await Mediator.Send(new GetRtoTaxSlabsQuery(stateName));
        return HandleResult(result);
    }

    /// <summary>
    /// Creates a new state and fuel-type specific RTO tax slab.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRtoTaxSlab([FromBody] CreateRtoTaxSlabCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Updates tax percentage and cess for an existing RTO slab.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRtoTaxSlab(Guid id, [FromBody] UpdateRtoTaxSlabCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Deletes an RTO tax slab.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRtoTaxSlab(Guid id)
    {
        var result = await Mediator.Send(new DeleteRtoTaxSlabCommand(id));
        return HandleResult(result);
    }
}
