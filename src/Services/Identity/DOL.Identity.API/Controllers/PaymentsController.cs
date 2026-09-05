using DOL.Identity.Application.Commands.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class PaymentsController : ApiControllerBase
{
    /// <summary>
    /// Initiates an online token booking, down payment, or accessory purchase payment order.
    /// </summary>
    [HttpPost("initiate")]
    public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Verifies gateway payment signature and marks transaction successful.
    /// </summary>
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Processes a payment refund for cancelled bookings or orders.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager")]
    [HttpPost("{id:guid}/refund")]
    public async Task<IActionResult> RefundPayment(Guid id, [FromQuery] string? reason)
    {
        var result = await Mediator.Send(new RefundPaymentCommand(id, reason));
        return HandleResult(result);
    }

    /// <summary>
    /// Queries transactions ledger, optionally filtered by buyer ID or order ID.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPayments([FromQuery] Guid? buyerId, [FromQuery] Guid? orderId)
    {
        var result = await Mediator.Send(new GetPaymentsQuery(buyerId, orderId));
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves payment transaction details and receipt by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPaymentById(Guid id)
    {
        var result = await Mediator.Send(new GetPaymentByIdQuery(id));
        return HandleResult(result);
    }
}
