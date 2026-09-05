using System.Security.Cryptography;
using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<string>>
{
    private readonly IIdentityDbContext _context;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(IIdentityDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (user == null)
        {
            // For security, don't reveal if user exists or not, but return success indication
            return Result.Success("If the email is registered, a password reset token has been sent.");
        }

        // Generate 6-digit OTP / random token
        var resetToken = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        user.SetPasswordResetToken(resetToken, TimeSpan.FromMinutes(15));

        await _context.SaveChangesAsync(cancellationToken);

        await _emailService.SendEmailAsync(
            user.Email,
            "DOL Password Reset Code",
            $"Your password reset code is: {resetToken}. This code will expire in 15 minutes.",
            cancellationToken
        );

        return Result.Success(resetToken);
    }
}
