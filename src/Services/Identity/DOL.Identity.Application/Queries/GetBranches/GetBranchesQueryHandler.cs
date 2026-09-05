using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Queries.GetBranches;

public class GetBranchesQueryHandler : IRequestHandler<GetBranchesQuery, Result<List<BranchDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetBranchesQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<BranchDto>>> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Branches
            .Include(b => b.City)
                .ThenInclude(c => c!.StateRegion)
                    .ThenInclude(s => s!.Country)
            .AsNoTracking();

        if (request.CityId.HasValue)
        {
            query = query.Where(b => b.CityId == request.CityId.Value);
        }

        if (request.ActiveOnly.HasValue && request.ActiveOnly.Value)
        {
            query = query.Where(b => b.IsActive);
        }

        var branches = await query
            .OrderBy(b => b.Name)
            .Select(b => new BranchDto(
                b.Id,
                b.CompanyId,
                b.CityId,
                b.Name,
                b.BranchCode,
                b.Address,
                b.ContactPhone,
                b.ContactEmail,
                b.IsActive,
                b.IsMainBranch,
                b.CreatedAt,
                b.City != null ? b.City.Name : null,
                b.City != null && b.City.StateRegion != null ? b.City.StateRegion.Name : null,
                b.City != null && b.City.StateRegion != null && b.City.StateRegion.Country != null ? b.City.StateRegion.Country.Name : null
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(branches);
    }
}
