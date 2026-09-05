using DOL.Identity.Application.DTOs;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using MediatR;

namespace DOL.Identity.Application.Commands.CreateScopedUser;

public record CreateScopedUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string Role,
    AccessScope Scope,
    Guid? BranchId = null,
    Guid? ScopeEntityId = null
) : IRequest<Result<UserDto>>;
