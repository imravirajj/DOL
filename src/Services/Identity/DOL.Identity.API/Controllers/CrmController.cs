using DOL.Identity.Application.Commands.Crm;
using DOL.Identity.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager,SalesExecutive")]
public class CrmController : ApiControllerBase
{
    /// <summary>
    /// Captures a new sales inquiry lead from showroom walk-in, website, or campaign.
    /// </summary>
    [HttpPost("leads")]
    public async Task<IActionResult> CreateLead([FromBody] CreateLeadCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Lists CRM pipeline leads, optionally filtered by assigned staff or stage.
    /// </summary>
    [HttpGet("leads")]
    public async Task<IActionResult> GetLeads([FromQuery] Guid? assignedStaffId, [FromQuery] LeadStage? stage)
    {
        var result = await Mediator.Send(new GetLeadsQuery(assignedStaffId, stage));
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves lead details and history by ID.
    /// </summary>
    [HttpGet("leads/{id:guid}")]
    public async Task<IActionResult> GetLeadById(Guid id)
    {
        var result = await Mediator.Send(new GetLeadByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Assigns or reassigns lead to a dealership sales executive.
    /// </summary>
    [HttpPut("leads/{id:guid}/assign")]
    public async Task<IActionResult> AssignLead(Guid id, [FromBody] Guid staffId)
    {
        var result = await Mediator.Send(new AssignLeadCommand(id, staffId));
        return HandleResult(result);
    }

    /// <summary>
    /// Advances lead through the sales pipeline stages (Contacted, TestDrive, Quotation, Won, Lost).
    /// </summary>
    [HttpPut("leads/{id:guid}/stage")]
    public async Task<IActionResult> UpdateLeadStage(Guid id, [FromBody] UpdateLeadStageCommand command)
    {
        if (id != command.LeadId) return BadRequest(new { errors = new[] { "Route ID does not match body LeadId." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Schedules a follow-up call or reminder for a sales lead.
    /// </summary>
    [HttpPost("leads/{id:guid}/follow-up")]
    public async Task<IActionResult> ScheduleFollowUp(Guid id, [FromBody] ScheduleFollowUpCommand command)
    {
        if (id != command.LeadId) return BadRequest(new { errors = new[] { "Route ID does not match body LeadId." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Archives or deletes a sales lead.
    /// </summary>
    [HttpDelete("leads/{id:guid}")]
    public async Task<IActionResult> DeleteLead(Guid id)
    {
        var result = await Mediator.Send(new DeleteLeadCommand(id));
        return HandleResult(result);
    }
}
