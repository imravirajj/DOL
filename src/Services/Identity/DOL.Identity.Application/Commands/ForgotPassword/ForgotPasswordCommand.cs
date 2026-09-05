using DOL.SharedKernel;
using MediatR;

namespace DOL.Identity.Application.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result<string>>;
