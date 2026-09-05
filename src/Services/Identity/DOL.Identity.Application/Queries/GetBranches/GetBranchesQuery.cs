using DOL.Identity.Application.DTOs;
using DOL.SharedKernel;
using MediatR;

namespace DOL.Identity.Application.Queries.GetBranches;

public record GetBranchesQuery(
    Guid? CityId = null,
    bool? ActiveOnly = true
) : IRequest<Result<List<BranchDto>>>;
