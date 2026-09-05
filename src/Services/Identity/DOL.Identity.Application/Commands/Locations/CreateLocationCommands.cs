using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Locations;

public record CreateCountryCommand(string Name, string IsoCode) : IRequest<Result<CountryDto>>;

public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, Result<CountryDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;

    public CreateCountryCommandHandler(IIdentityDbContext context, ICurrentUserContext currentUserContext)
    {
        _context = context;
        _currentUserContext = currentUserContext;
    }

    public async Task<Result<CountryDto>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserContext.CompanyId.HasValue)
            return Result.Failure<CountryDto>("Tenant context missing.");

        var companyId = _currentUserContext.CompanyId.Value;
        var isoCode = request.IsoCode.Trim().ToUpperInvariant();

        var exists = await _context.Countries
            .AnyAsync(c => c.CompanyId == companyId && c.IsoCode == isoCode, cancellationToken);
        if (exists)
            return Result.Failure<CountryDto>($"Country with code '{isoCode}' already exists for this company.");

        var country = new Country(companyId, request.Name, isoCode);
        _context.Countries.Add(country);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new CountryDto(country.Id, country.CompanyId, country.Name, country.IsoCode));
    }
}

public record CreateStateRegionCommand(Guid CountryId, string Name, string? StateCode = null) : IRequest<Result<StateRegionDto>>;

public class CreateStateRegionCommandHandler : IRequestHandler<CreateStateRegionCommand, Result<StateRegionDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;

    public CreateStateRegionCommandHandler(IIdentityDbContext context, ICurrentUserContext currentUserContext)
    {
        _context = context;
        _currentUserContext = currentUserContext;
    }

    public async Task<Result<StateRegionDto>> Handle(CreateStateRegionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserContext.CompanyId.HasValue)
            return Result.Failure<StateRegionDto>("Tenant context missing.");

        var companyId = _currentUserContext.CompanyId.Value;
        var countryExists = await _context.Countries
            .AnyAsync(c => c.Id == request.CountryId && c.CompanyId == companyId, cancellationToken);
        if (!countryExists)
            return Result.Failure<StateRegionDto>("Country not found.");

        var state = new StateRegion(companyId, request.CountryId, request.Name, request.StateCode);
        _context.StateRegions.Add(state);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new StateRegionDto(state.Id, state.CompanyId, state.CountryId, state.Name, state.StateCode));
    }
}

public record CreateCityCommand(Guid StateRegionId, string Name) : IRequest<Result<CityDto>>;

public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, Result<CityDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;

    public CreateCityCommandHandler(IIdentityDbContext context, ICurrentUserContext currentUserContext)
    {
        _context = context;
        _currentUserContext = currentUserContext;
    }

    public async Task<Result<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserContext.CompanyId.HasValue)
            return Result.Failure<CityDto>("Tenant context missing.");

        var companyId = _currentUserContext.CompanyId.Value;
        var stateExists = await _context.StateRegions
            .AnyAsync(s => s.Id == request.StateRegionId && s.CompanyId == companyId, cancellationToken);
        if (!stateExists)
            return Result.Failure<CityDto>("State/Region not found.");

        var city = new City(companyId, request.StateRegionId, request.Name);
        _context.Cities.Add(city);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new CityDto(city.Id, city.CompanyId, city.StateRegionId, city.Name));
    }
}
