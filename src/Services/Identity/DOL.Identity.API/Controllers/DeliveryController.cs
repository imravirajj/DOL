using DOL.Identity.Application.Commands.Delivery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class DeliveryController : ApiControllerBase
{
    /// <summary>
    /// Submits a digital 50-point Pre-Delivery Inspection (PDI) checklist report.
    /// </summary>
    [HttpPost("pdi")]
    [Authorize(Roles = "Admin,CompanyAdmin,BranchManager")]
    public async Task<IActionResult> SubmitPdi([FromBody] SubmitPdiCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves the PDI inspection report for a vehicle order.
    /// </summary>
    [HttpGet("pdi/{orderId:guid}")]
    public async Task<IActionResult> GetPdiReport(Guid orderId)
    {
        var result = await Mediator.Send(new GetPdiReportQuery(orderId));
        return HandleResult(result);
    }

    /// <summary>
    /// Generates and sends a 6-digit delivery verification OTP to the customer's phone.
    /// </summary>
    [HttpPost("generate-otp/{orderId:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin,BranchManager")]
    public async Task<IActionResult> GenerateDeliveryOtp(Guid orderId)
    {
        var result = await Mediator.Send(new GenerateDeliveryOtpCommand(orderId));
        return HandleResult(result);
    }
}
