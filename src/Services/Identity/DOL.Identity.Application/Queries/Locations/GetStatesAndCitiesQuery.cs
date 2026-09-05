using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Queries.Locations;

public record GetStatesQuery(Guid? CountryId = null) : IRequest<Result<List<StateRegionDto>>>;

public class GetStatesQueryHandler : IRequestHandler<GetStatesQuery, Result<List<StateRegionDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetStatesQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<StateRegionDto>>> Handle(GetStatesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StateRegions.AsNoTracking();

        if (request.CountryId.HasValue)
        {
            query = query.Where(s => s.CountryId == request.CountryId.Value);
        }

        var states = await query
            .OrderBy(s => s.Name)
            .Select(s => new StateRegionDto(s.Id, s.CompanyId, s.CountryId, s.Name, s.StateCode))
            .ToListAsync(cancellationToken);

        return Result<List<StateRegionDto>>.Success(states);
    }
}

public record GetCitiesQuery(Guid? StateRegionId = null) : IRequest<Result<List<CityDto>>>;

public class GetCitiesQueryHandler : IRequestHandler<GetCitiesQuery, Result<List<CityDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetCitiesQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CityDto>>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Cities.AsNoTracking();

        if (request.StateRegionId.HasValue)
        {
            query = query.Where(c => c.StateRegionId == request.StateRegionId.Value);
        }

        var cities = await query
            .OrderBy(c => c.Name)
            .Select(c => new CityDto(c.Id, c.CompanyId, c.StateRegionId, c.Name))
            .ToListAsync(cancellationToken);

        return Result<List<CityDto>>.Success(cities);
    }
}
