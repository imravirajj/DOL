using DOL.Identity.Application.Commands.Quotations;
using DOL.Identity.Application.Queries.Quotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Route("api/[controller]")]
[Route("api/quotation")]
public class QuotationController : ApiControllerBase
{
    /// <summary>
    /// Generates a real-time digital on-road quotation with state-wise RTO tax, insurance add-ons, and accessories.
    /// Price is locked for 7 days.
    /// </summary>
    [HttpPost("generate")]
    [AllowAnonymous]
    public async Task<IActionResult> GenerateQuotation([FromBody] GenerateQuotationCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves full breakdown details of a specific quotation.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetQuotationById(Guid id)
    {
        var result = await Mediator.Send(new GetQuotationByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Returns quotations history (branch staff sees their branch; buyers see their quotations; HQ sees all).
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetQuotations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await Mediator.Send(new GetQuotationsQuery(pageNumber, pageSize));
        return HandleResult(result);
    }

    /// <summary>
    /// Accepts quotation and initiates a 15-minute exclusive vehicle reservation hold.
    /// </summary>
    [HttpPost("{id:guid}/book")]
    [Authorize]
    public async Task<IActionResult> BookFromQuotation(Guid id)
    {
        var result = await Mediator.Send(new ConvertQuotationToBookingCommand(id));
        return HandleResult(result);
    }
}
