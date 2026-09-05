using DOL.Identity.Application.Commands.Companies;
using DOL.Identity.Application.Commands.RegisterCompany;
using DOL.Identity.Application.Queries.Companies;
using DOL.Identity.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

public class CompanyController : ApiControllerBase
{
    /// <summary>
    /// Registers a new enterprise automotive dealership company with its root admin.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Lists all companies with pagination and optional status filtering.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCompanies([FromQuery] CompanyStatus? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await Mediator.Send(new GetCompaniesQuery(status, pageNumber, pageSize));
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves company details and branch count by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> GetCompanyById(Guid id)
    {
        var result = await Mediator.Send(new GetCompanyByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Updates company profile, phone number, address, currency, and time zone.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        }

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Suspends or deactivates a company account.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        var result = await Mediator.Send(new DeleteCompanyCommand(id));
        return HandleResult(result);
    }
}
