using DOL.Identity.Application.Commands.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class ReviewsController : ApiControllerBase
{
    /// <summary>
    /// Submits a customer review and 1-5 star dealership rating.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Lists public dealership reviews, optionally filtered by branch.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetReviews([FromQuery] Guid? branchId)
    {
        var result = await Mediator.Send(new GetDealershipReviewsQuery(branchId));
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves a single review by ID.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetReviewById(Guid id)
    {
        var result = await Mediator.Send(new GetReviewByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Dealership manager submits public response to a customer review.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager")]
    [HttpPost("{id:guid}/respond")]
    public async Task<IActionResult> RespondToReview(Guid id, [FromBody] string response)
    {
        var result = await Mediator.Send(new RespondToReviewCommand(id, response));
        return HandleResult(result);
    }
}
