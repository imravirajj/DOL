using DOL.SharedKernel;
using MediatR;

namespace DOL.Identity.Application.Commands.AssignRole;

public record AssignRoleCommand(Guid UserId, string RoleName) : IRequest<Result>;
