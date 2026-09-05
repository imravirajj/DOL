using DOL.Identity.Application.Commands.TestDrives;
using DOL.Identity.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class TestDriveController : ApiControllerBase
{
    /// <summary>
    /// Books a vehicle test drive slot at showroom or doorstep.
    /// </summary>
    [HttpPost("book")]
    public async Task<IActionResult> BookTestDrive([FromBody] BookTestDriveCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Lists test drive bookings, optionally filtered by buyer ID or status.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTestDrives([FromQuery] Guid? buyerId, [FromQuery] TestDriveStatus? status)
    {
        var result = await Mediator.Send(new GetTestDrivesQuery(buyerId, status));
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves test drive booking details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTestDriveById(Guid id)
    {
        var result = await Mediator.Send(new GetTestDriveByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Reschedules or completes a test drive with customer rating and feedback notes.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTestDrive(Guid id, [FromBody] UpdateTestDriveCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Cancels a test drive booking.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTestDrive(Guid id)
    {
        var result = await Mediator.Send(new DeleteTestDriveCommand(id));
        return HandleResult(result);
    }
}
