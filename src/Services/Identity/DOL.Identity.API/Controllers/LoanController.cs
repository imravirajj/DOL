using DOL.Identity.Application.Commands.Loans;
using DOL.Identity.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class LoanController : ApiControllerBase
{
    /// <summary>
    /// Calculates equated monthly installment (EMI), total interest, and total payable amount.
    /// </summary>
    [HttpPost("calculate-emi")]
    [AllowAnonymous]
    public async Task<IActionResult> CalculateEmi([FromBody] CalculateEmiQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Submits a digital auto-loan application linked to a car quotation.
    /// </summary>
    [HttpPost("apply")]
    public async Task<IActionResult> ApplyForLoan([FromBody] ApplyLoanCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Lists vehicle loan applications, optionally filtered by buyer ID or status.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetLoans([FromQuery] Guid? buyerId, [FromQuery] LoanStatus? status)
    {
        var result = await Mediator.Send(new GetLoansQuery(buyerId, status));
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves a single loan application by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetLoanById(Guid id)
    {
        var result = await Mediator.Send(new GetLoanByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Bank Desk issues in-principle digital sanction with approved amount and interest rate.
    /// </summary>
    [HttpPost("{id:guid}/sanction")]
    [Authorize(Roles = "Admin,CompanyAdmin,BranchManager")]
    public async Task<IActionResult> SanctionLoan(Guid id, [FromBody] SanctionLoanCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Updates loan required amount or tenure.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateLoan(Guid id, [FromBody] UpdateLoanCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Cancels or rejects a loan application.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin,BranchManager")]
    public async Task<IActionResult> DeleteLoan(Guid id)
    {
        var result = await Mediator.Send(new DeleteLoanCommand(id));
        return HandleResult(result);
    }
}
