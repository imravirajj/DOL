using DOL.Identity.Application.Commands.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class NotificationsController : ApiControllerBase
{
    /// <summary>
    /// Retrieves in-app alerts and notifications for the specified user.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] Guid userId, [FromQuery] bool? unreadOnly)
    {
        var result = await Mediator.Send(new GetMyNotificationsQuery(userId, unreadOnly));
        return HandleResult(result);
    }

    /// <summary>
    /// Sends an in-app or SMS alert to a customer or staff member.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager,SalesExecutive")]
    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await Mediator.Send(new MarkNotificationAsReadCommand(id));
        return HandleResult(result);
    }
}
