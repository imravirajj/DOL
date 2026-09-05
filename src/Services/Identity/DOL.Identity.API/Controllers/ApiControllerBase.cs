using DOL.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return Ok();

        return BadRequest(new { errors = result.Errors });
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(new { errors = result.Errors });
    }
}
