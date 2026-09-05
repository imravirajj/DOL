using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Queries.Locations;

public record GetLocationsQuery : IRequest<Result<List<CountryDto>>>;

public class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, Result<List<CountryDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetLocationsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CountryDto>>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        var countries = await _context.Countries
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CountryDto(c.Id, c.CompanyId, c.Name, c.IsoCode))
            .ToListAsync(cancellationToken);

        return Result.Success(countries);
    }
}
