using DOL.Identity.Application.Commands.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class DocumentsController : ApiControllerBase
{
    /// <summary>
    /// Uploads customer identity, address proof, or income KYC documents for RTO/loan verification.
    /// </summary>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadDocument([FromBody] UploadDocumentCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Queries uploaded KYC documents, optionally filtered by user ID or order ID.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetDocuments([FromQuery] Guid? userId, [FromQuery] Guid? orderId)
    {
        var result = await Mediator.Send(new GetDocumentsQuery(userId, orderId));
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves single document metadata and vault URL by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDocumentById(Guid id)
    {
        var result = await Mediator.Send(new GetDocumentByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Staff approves or rejects KYC document with verification status and remarks.
    /// </summary>
    [Authorize(Roles = "GlobalAdmin,SuperAdmin,CompanyAdmin,BranchManager,SalesExecutive")]
    [HttpPut("{id:guid}/verify")]
    public async Task<IActionResult> VerifyDocument(Guid id, [FromBody] VerifyDocumentCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Deletes or replaces an uploaded KYC document.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDocument(Guid id)
    {
        var result = await Mediator.Send(new DeleteDocumentCommand(id));
        return HandleResult(result);
    }
}
