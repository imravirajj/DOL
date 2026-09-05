using DOL.SharedKernel;
using MediatR;

namespace DOL.Identity.Application.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string ResetToken,
    string NewPassword
) : IRequest<Result>;
