using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IIdentityDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(IIdentityDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            return Result.Failure("User not found.");

        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            return Result.Failure("Current password is incorrect.");

        var newHash = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatePassword(newHash);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
