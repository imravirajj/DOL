using DOL.Identity.Application.Commands.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class BookingController : ApiControllerBase
{
    /// <summary>
    /// Locks a specific vehicle for 15 minutes exclusively for the current buyer.
    /// Prevents 2 or more buyers from booking the same vehicle.
    /// </summary>
    [HttpPost("reserve")]
    public async Task<IActionResult> ReserveVehicle([FromBody] ReserveVehicleCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Confirms permanent booking and locks VIN after booking payment succeeds.
    /// Protected with client idempotency key to prevent duplicate booking.
    /// </summary>
    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmBooking([FromBody] ConfirmBookingCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Joins the FIFO Priority Waitlist Queue when stock is 0.
    /// Assigns a transparent Token # and estimated factory arrival time.
    /// </summary>
    [HttpPost("waitlist")]
    public async Task<IActionResult> JoinWaitlist([FromBody] JoinWaitlistCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// 1-Click cancellation with 100% full refund.
    /// Releases the held car or re-allocates it immediately to the next customer in the waitlist queue.
    /// </summary>
    [HttpPost("cancel")]
    public async Task<IActionResult> CancelBooking([FromBody] CancelWaitlistOrBookingCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
