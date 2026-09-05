using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Events;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.CreateBranch;

public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, Result<BranchDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;

    public CreateBranchCommandHandler(
        IIdentityDbContext context,
        ICurrentUserContext currentUserContext)
    {
        _context = context;
        _currentUserContext = currentUserContext;
    }

    public async Task<Result<BranchDto>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserContext.CompanyId.HasValue)
        {
            return Result.Failure<BranchDto>("Tenant context is missing. Cannot create branch.");
        }

        var companyId = _currentUserContext.CompanyId.Value;

        // Verify city exists
        var city = await _context.Cities
            .Include(c => c.StateRegion)
                .ThenInclude(s => s!.Country)
            .FirstOrDefaultAsync(c => c.Id == request.CityId && c.CompanyId == companyId, cancellationToken);

        if (city == null)
        {
            return Result.Failure<BranchDto>("Specified city does not exist for this company.");
        }

        var normalizedCode = request.BranchCode.Trim().ToUpperInvariant();
        var codeExists = await _context.Branches
            .AnyAsync(b => b.CompanyId == companyId && b.BranchCode == normalizedCode, cancellationToken);

        if (codeExists)
        {
            return Result.Failure<BranchDto>($"Branch with code '{normalizedCode}' already exists in this company.");
        }

        var branch = new Branch(
            companyId,
            city.Id,
            request.Name,
            normalizedCode,
            request.Address,
            request.ContactPhone,
            request.ContactEmail,
            request.IsMainBranch
        );

        branch.AddDomainEvent(new BranchCreatedEvent(branch.Id, companyId, branch.Name, branch.BranchCode));

        _context.Branches.Add(branch);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new BranchDto(
            branch.Id,
            branch.CompanyId,
            branch.CityId,
            branch.Name,
            branch.BranchCode,
            branch.Address,
            branch.ContactPhone,
            branch.ContactEmail,
            branch.IsActive,
            branch.IsMainBranch,
            branch.CreatedAt,
            city.Name,
            city.StateRegion?.Name,
            city.StateRegion?.Country?.Name
        );

        return Result.Success(dto);
    }
}
