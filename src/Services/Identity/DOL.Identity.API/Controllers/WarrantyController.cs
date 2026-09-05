using DOL.Identity.Application.Commands.Warranty;
using DOL.Identity.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class WarrantyController : ApiControllerBase
{
    /// <summary>
    /// Browses extended warranty, AMC, and roadside assistance packages.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("packages")]
    public async Task<IActionResult> GetWarrantyPackages([FromQuery] WarrantyPackageType? packageType)
    {
        var result = await Mediator.Send(new GetWarrantyPackagesQuery(packageType));
        return HandleResult(result);
    }

    /// <summary>
    /// Registers a new extended warranty or AMC package definition.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin")]
    [HttpPost("packages")]
    public async Task<IActionResult> CreateWarrantyPackage([FromBody] CreateWarrantyPackageCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Purchases and activates a warranty or AMC package for a vehicle.
    /// </summary>
    [HttpPost("subscribe")]
    public async Task<IActionResult> SubscribeWarranty([FromBody] SubscribeWarrantyCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Queries active vehicle warranty subscriptions, optionally by buyer ID or VIN.
    /// </summary>
    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetSubscriptions([FromQuery] Guid? buyerId, [FromQuery] string? vin)
    {
        var result = await Mediator.Send(new GetMyWarrantySubscriptionsQuery(buyerId, vin));
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves warranty subscription certificate by ID.
    /// </summary>
    [HttpGet("subscriptions/{id:guid}")]
    public async Task<IActionResult> GetSubscriptionById(Guid id)
    {
        var result = await Mediator.Send(new GetWarrantySubscriptionByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Cancels a warranty subscription.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager")]
    [HttpDelete("subscriptions/{id:guid}")]
    public async Task<IActionResult> CancelSubscription(Guid id)
    {
        var result = await Mediator.Send(new CancelWarrantySubscriptionCommand(id));
        return HandleResult(result);
    }
}
