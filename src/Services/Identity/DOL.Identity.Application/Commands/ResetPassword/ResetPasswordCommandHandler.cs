using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IIdentityDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(IIdentityDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (user == null || !user.ValidatePasswordResetToken(request.ResetToken))
        {
            return Result.Failure("Invalid or expired password reset token.");
        }

        var newHash = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatePassword(newHash);
        user.ClearPasswordResetToken();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
