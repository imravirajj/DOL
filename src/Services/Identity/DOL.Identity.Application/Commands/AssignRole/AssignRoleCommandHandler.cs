using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.AssignRole;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public AssignRoleCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            return Result.Failure($"User with ID '{request.UserId}' not found.");

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == request.RoleName, cancellationToken);

        if (role == null)
            return Result.Failure($"Role '{request.RoleName}' does not exist.");

        user.AddRole(role.Id);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
