using DOL.Identity.Application.Commands.Orders;
using DOL.Identity.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class OrderController : ApiControllerBase
{
    /// <summary>
    /// Converts a price-locked quotation into a confirmed vehicle order upon token payment.
    /// </summary>
    [HttpPost("create-from-quotation")]
    public async Task<IActionResult> CreateOrderFromQuotation([FromBody] CreateOrderFromQuotationCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Lists vehicle orders, optionally filtered by buyer ID or order status.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] Guid? buyerId, [FromQuery] OrderStatus? status)
    {
        var result = await Mediator.Send(new GetOrdersQuery(buyerId, status));
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves order lifecycle status, VIN allocation, and delivery schedule.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var result = await Mediator.Send(new GetOrderByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Advances order through the state machine (VinAllocated, DownPaymentReceived, RtoCompleted, PdiReady).
    /// </summary>
    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Admin,CompanyAdmin,BranchManager")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Verifies the customer's 6-digit Delivery OTP and marks the order as Delivered.
    /// </summary>
    [HttpPost("{id:guid}/verify-delivery-otp")]
    [Authorize(Roles = "Admin,CompanyAdmin,BranchManager")]
    public async Task<IActionResult> VerifyDeliveryOtp(Guid id, [FromBody] string otp)
    {
        var result = await Mediator.Send(new VerifyDeliveryOtpCommand(id, otp));
        return HandleResult(result);
    }

    /// <summary>
    /// Cancels a vehicle order.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        var result = await Mediator.Send(new DeleteOrderCommand(id));
        return HandleResult(result);
    }
}
