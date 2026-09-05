using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Queries.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserDto>>
{
    private readonly IIdentityDbContext _context;

    public GetUserProfileQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            return Result.Failure<UserDto>("User not found.");

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        var userDto = new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber,
            user.Status.ToString(),
            user.EmailConfirmed,
            roles,
            user.CreatedAt
        );

        return Result.Success(userDto);
    }
}
