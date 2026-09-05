using DOL.Identity.Application.Commands.CreateBranch;
using DOL.Identity.Application.Queries.GetBranches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class BranchController : ApiControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetBranches([FromQuery] Guid? cityId, [FromQuery] bool? activeOnly = true)
    {
        var query = new GetBranchesQuery(cityId, activeOnly);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }
}
