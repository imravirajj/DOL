using DOL.Identity.Application.Commands.RegisterCompany;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

[Route("api/[controller]")]
[Route("api/companies")]
public class CompanyController : ApiControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
