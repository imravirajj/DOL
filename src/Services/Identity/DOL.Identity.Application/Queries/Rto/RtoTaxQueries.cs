using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Queries.Rto;

public record GetRtoTaxSlabsQuery(string? StateName = null) : IRequest<Result<List<RtoTaxSlabDto>>>;

public class GetRtoTaxSlabsQueryHandler : IRequestHandler<GetRtoTaxSlabsQuery, Result<List<RtoTaxSlabDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetRtoTaxSlabsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<RtoTaxSlabDto>>> Handle(GetRtoTaxSlabsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.RtoTaxSlabs.AsNoTracking();

        if (!string.IsNullOrEmpty(request.StateName))
        {
            query = query.Where(r => r.StateName.ToLower() == request.StateName.Trim().ToLower());
        }

        var slabs = await query
            .OrderBy(r => r.StateName)
            .ThenBy(r => r.FuelType)
            .Select(r => new RtoTaxSlabDto(
                r.Id,
                r.CompanyId,
                r.StateName,
                r.FuelType,
                r.TaxPercentage,
                r.CessPercentage))
            .ToListAsync(cancellationToken);

        return Result<List<RtoTaxSlabDto>>.Success(slabs);
    }
}
