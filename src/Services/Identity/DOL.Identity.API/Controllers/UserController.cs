using System.Security.Claims;
using DOL.Identity.Application.Commands.AssignRole;
using DOL.Identity.Application.Commands.Users;
using DOL.Identity.Application.Queries.GetAllUsers;
using DOL.Identity.Application.Queries.GetUserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class UserController : ApiControllerBase
{
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await Mediator.Send(new GetUserProfileQuery(userId));
        return HandleResult(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await Mediator.Send(new GetAllUsersQuery(pageNumber, pageSize));
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin,BranchManager")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await Mediator.Send(new GetUserByIdQuery(id));
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin,BranchManager")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await Mediator.Send(new DeleteUserCommand(id));
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/assign-role")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> AssignRole(Guid id, [FromBody] string roleName)
    {
        var result = await Mediator.Send(new AssignRoleCommand(id, roleName));
        return HandleResult(result);
    }

    [HttpPost("create-scoped")]
    [Authorize(Roles = "Admin,CompanyAdmin,BranchManager")]
    public async Task<IActionResult> CreateScopedUser([FromBody] DOL.Identity.Application.Commands.CreateScopedUser.CreateScopedUserCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
