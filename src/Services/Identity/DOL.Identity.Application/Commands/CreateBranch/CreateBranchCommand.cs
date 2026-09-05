using DOL.Identity.Application.DTOs;
using DOL.SharedKernel;
using MediatR;

namespace DOL.Identity.Application.Commands.CreateBranch;

public record CreateBranchCommand(
    string Name,
    string BranchCode,
    string Address,
    Guid CityId,
    string? ContactPhone = null,
    string? ContactEmail = null,
    bool IsMainBranch = false
) : IRequest<Result<BranchDto>>;
