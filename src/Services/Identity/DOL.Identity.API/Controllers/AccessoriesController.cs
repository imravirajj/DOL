using DOL.Identity.Application.Commands.Accessories;
using DOL.Identity.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class AccessoriesController : ApiControllerBase
{
    /// <summary>
    /// Browses vehicle accessories catalog, optionally filtered by vehicle variant or category.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAccessories([FromQuery] Guid? compatibleVariantId, [FromQuery] AccessoryCategory? category)
    {
        var result = await Mediator.Send(new GetAccessoriesQuery(compatibleVariantId, category));
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves accessory details by ID.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAccessoryById(Guid id)
    {
        var result = await Mediator.Send(new GetAccessoryByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Adds a new OEM accessory to the dealership store.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager")]
    [HttpPost]
    public async Task<IActionResult> CreateAccessory([FromBody] CreateAccessoryCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Updates an accessory item in the catalog.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAccessory(Guid id, [FromBody] UpdateAccessoryCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Removes an accessory from the dealership catalog.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAccessory(Guid id)
    {
        var result = await Mediator.Send(new DeleteAccessoryCommand(id));
        return HandleResult(result);
    }
}
