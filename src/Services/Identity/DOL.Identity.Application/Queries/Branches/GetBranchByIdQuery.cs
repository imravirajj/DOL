using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Queries.Branches;

public record GetBranchByIdQuery(Guid Id) : IRequest<Result<BranchDto>>;

public class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, Result<BranchDto>>
{
    private readonly IIdentityDbContext _context;

    public GetBranchByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<BranchDto>> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
    {
        var branch = await _context.Branches
            .Include(b => b.City)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (branch == null)
        {
            return Result<BranchDto>.Failure("Branch not found.");
        }

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
            branch.City?.Name);

        return Result<BranchDto>.Success(dto);
    }
}
