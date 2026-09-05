using DOL.Identity.Application.DTOs;
using DOL.SharedKernel;
using MediatR;

namespace DOL.Identity.Application.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserDto>>;
