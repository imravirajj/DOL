using DOL.Identity.Application.DTOs;
using DOL.SharedKernel;
using MediatR;

namespace DOL.Identity.Application.Queries.GetAllUsers;

public record GetAllUsersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<List<UserDto>>>;
