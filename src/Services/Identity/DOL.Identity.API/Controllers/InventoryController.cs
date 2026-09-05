using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Application.Queries.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.API.Controllers;

[Route("api/[controller]")]
[Route("api/inventory")]
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
    /// Searches available stock in sibling branches when the local branch is out of stock.
    /// </summary>
    [HttpGet("inter-branch")]
    public async Task<IActionResult> FindInterBranchStock([FromQuery] Guid variantId, [FromQuery] Guid currentBranchId)
    {
        var result = await Mediator.Send(new FindInterBranchStockQuery(variantId, currentBranchId));
        return HandleResult(result);
    }
}
