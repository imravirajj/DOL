using DOL.Identity.Application.Commands.Inventory;
using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Application.Queries.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.API.Controllers;

[Authorize]
public class InventoryController : ApiControllerBase
{
    private readonly IIdentityDbContext _context;

    public InventoryController(IIdentityDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns live vehicle stock in the current branch (respects branch data isolation).
    /// </summary>
    [HttpGet("stock")]
    public async Task<IActionResult> GetStock([FromQuery] Guid? variantId)
    {
        var query = _context.VehicleStocks
            .Include(s => s.Branch)
            .Include(s => s.VehicleVariant)
                .ThenInclude(v => v!.VehicleModel)
            .AsNoTracking();

        if (variantId.HasValue)
        {
            query = query.Where(s => s.VehicleVariantId == variantId.Value);
        }

        var list = await query
            .OrderBy(s => s.VehicleVariant!.VariantName)
            .Select(s => new VehicleStockDto(
                s.Id,
                s.CompanyId,
                s.BranchId,
                s.VehicleVariantId,
                s.VinNumber,
                s.EngineNumber,
                s.Color,
                s.Status.ToString(),
                s.Branch != null ? s.Branch.Name : null,
                s.VehicleVariant != null ? s.VehicleVariant.VariantName : null,
                s.VehicleVariant != null && s.VehicleVariant.VehicleModel != null ? s.VehicleVariant.VehicleModel.Make : null,
                s.VehicleVariant != null && s.VehicleVariant.VehicleModel != null ? s.VehicleVariant.VehicleModel.Model : null,
                s.VehicleVariant != null ? s.VehicleVariant.ExShowroomPrice : 0,
                false,
                s.ReservationExpiresAt
            ))
            .ToListAsync();

        return Ok(list);
    }

    /// <summary>
    /// Retrieves a single vehicle stock unit by ID.
    /// </summary>
    [HttpGet("stock/{id:guid}")]
    public async Task<IActionResult> GetStockById(Guid id)
    {
        var result = await Mediator.Send(new GetVehicleStockByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Inwards a new vehicle unit into inventory with unique 17-character VIN.
    /// </summary>
    [HttpPost("stock")]
    [Authorize(Roles = "Admin,CompanyAdmin,BranchManager")]
    public async Task<IActionResult> AddStock([FromBody] AddVehicleStockCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Updates stock details (color, engine number, or operational status).
    /// </summary>
    [HttpPut("stock/{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin,BranchManager")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateVehicleStockCommand command)
    {
        if (id != command.Id) return BadRequest(new { errors = new[] { "Route ID does not match body ID." } });
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Removes a defective or returned vehicle unit from stock.
    /// </summary>
    [HttpDelete("stock/{id:guid}")]
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public async Task<IActionResult> DeleteStock(Guid id)
    {
        var result = await Mediator.Send(new DeleteVehicleStockCommand(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Searches available stock in sibling branches when the local branch is out of stock.
    /// </summary>
    [HttpGet("inter-branch")]
    public async Task<IActionResult> FindInterBranchStock([FromQuery] Guid variantId, [FromQuery] Guid currentBranchId)
    {
        var result = await Mediator.Send(new FindInterBranchStockQuery(variantId, currentBranchId));
        return HandleResult(result);
    }
}
