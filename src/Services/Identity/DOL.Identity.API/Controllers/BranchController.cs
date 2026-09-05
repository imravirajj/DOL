using DOL.Identity.Application.Commands.Branches;
using DOL.Identity.Application.Commands.CreateBranch;
using DOL.Identity.Application.Queries.Branches;
using DOL.Identity.Application.Queries.GetBranches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class BranchController : ApiControllerBase
{
    /// <summary>
    /// Creates a new branch under the authenticated company.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Lists all branches filtered by city or active status.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBranches([FromQuery] Guid? cityId, [FromQuery] bool? activeOnly = true)
    {
        var query = new GetBranchesQuery(cityId, activeOnly);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves branch details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBranchById(Guid id)
    {
        var result = await Mediator.Send(new GetBranchByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Updates branch name, address, contact phone, or contact email.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> UpdateBranch(Guid id, [FromBody] UpdateBranchCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        }

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Deactivates a branch.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> DeleteBranch(Guid id)
    {
        var result = await Mediator.Send(new DeleteBranchCommand(id));
        return HandleResult(result);
    }
}
