using DOL.Identity.Application.Commands.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class ServiceController : ApiControllerBase
{
    /// <summary>
    /// Books a vehicle periodic maintenance or workshop repair appointment.
    /// </summary>
    [HttpPost("book")]
    public async Task<IActionResult> BookServiceAppointment([FromBody] BookServiceAppointmentCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Lists service appointments, optionally filtered by buyer ID.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetServiceAppointments([FromQuery] Guid? buyerId)
    {
        var result = await Mediator.Send(new GetServiceAppointmentsQuery(buyerId));
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves complete workshop service history and job cards by vehicle VIN.
    /// </summary>
    [HttpGet("history")]
    public async Task<IActionResult> GetServiceHistory([FromQuery] string vin)
    {
        if (string.IsNullOrWhiteSpace(vin)) return BadRequest(new { errors = new[] { "VIN parameter is required." } });
        var result = await Mediator.Send(new GetServiceHistoryByVinQuery(vin));
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves appointment details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetServiceAppointmentById(Guid id)
    {
        var result = await Mediator.Send(new GetServiceAppointmentByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Updates service appointment status and actual billing cost.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager,SalesExecutive")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateServiceAppointment(Guid id, [FromBody] UpdateServiceAppointmentCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Cancels a service appointment.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> CancelServiceAppointment(Guid id)
    {
        var result = await Mediator.Send(new CancelServiceAppointmentCommand(id));
        return HandleResult(result);
    }
}
