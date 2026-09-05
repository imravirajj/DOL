using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Locations;

// --- Country Commands ---
public record UpdateCountryCommand(Guid Id, string Name, string IsoCode) : IRequest<Result>;

public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateCountryCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (country == null) return Result.Failure("Country not found.");

        country.Update(request.Name, request.IsoCode);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteCountryCommand(Guid Id) : IRequest<Result>;

public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public DeleteCountryCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (country == null) return Result.Failure("Country not found.");

        _context.Countries.Remove(country);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// --- State Commands ---
public record UpdateStateCommand(Guid Id, string Name, string? StateCode) : IRequest<Result>;

public class UpdateStateCommandHandler : IRequestHandler<UpdateStateCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateStateCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
    {
        var state = await _context.StateRegions.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (state == null) return Result.Failure("State/Region not found.");

        state.Update(request.Name, request.StateCode);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteStateCommand(Guid Id) : IRequest<Result>;

public class DeleteStateCommandHandler : IRequestHandler<DeleteStateCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public DeleteStateCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteStateCommand request, CancellationToken cancellationToken)
    {
        var state = await _context.StateRegions.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (state == null) return Result.Failure("State/Region not found.");

        _context.StateRegions.Remove(state);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// --- City Commands ---
public record UpdateCityCommand(Guid Id, string Name) : IRequest<Result>;

public class UpdateCityCommandHandler : IRequestHandler<UpdateCityCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateCityCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (city == null) return Result.Failure("City not found.");

        city.Update(request.Name);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteCityCommand(Guid Id) : IRequest<Result>;

public class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public DeleteCityCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (city == null) return Result.Failure("City not found.");

        _context.Cities.Remove(city);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
