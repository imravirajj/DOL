using DOL.Identity.Application.Commands.Insurance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class InsuranceController : ApiControllerBase
{
    /// <summary>
    /// Compares digital insurance plans and coverage riders from top insurers based on vehicle price.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("plans")]
    public async Task<IActionResult> GetInsurancePlans([FromQuery] decimal exShowroomPrice = 1000000)
    {
        var result = await Mediator.Send(new GetInsurancePlansQuery(exShowroomPrice));
        return HandleResult(result);
    }

    /// <summary>
    /// Issues an active insurance policy against a booked vehicle order.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager,SalesExecutive")]
    [HttpPost("issue")]
    public async Task<IActionResult> IssueInsurancePolicy([FromBody] IssueInsurancePolicyCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves active digital insurance policy and coverage certificate for an order.
    /// </summary>
    [HttpGet("policy/{orderId:guid}")]
    public async Task<IActionResult> GetInsurancePolicyByOrder(Guid orderId)
    {
        var result = await Mediator.Send(new GetInsurancePolicyByOrderQuery(orderId));
        return HandleResult(result);
    }

    /// <summary>
    /// Cancels an insurance policy.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> CancelInsurancePolicy(Guid id)
    {
        var result = await Mediator.Send(new CancelInsurancePolicyCommand(id));
        return HandleResult(result);
    }
}
