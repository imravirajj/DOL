using DOL.SharedKernel;
using MediatR;

namespace DOL.Identity.Application.Commands.ChangePassword;

public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<Result>;
