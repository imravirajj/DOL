using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<UserDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetAllUsersQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var userDtos = users.Select(user => new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber,
            user.Status.ToString(),
            user.EmailConfirmed,
            user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            user.CreatedAt
        )).ToList();

        return Result.Success(userDtos);
    }
}
