using DOL.Identity.Application.DTOs;
using DOL.SharedKernel;
using MediatR;

namespace DOL.Identity.Application.Commands.Login;

public record LoginCommand(
    string Email,
    string Password,
    string ClientIp = "127.0.0.1"
) : IRequest<Result<AuthResultDto>>;
