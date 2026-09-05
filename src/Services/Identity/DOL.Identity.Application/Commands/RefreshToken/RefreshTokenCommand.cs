using DOL.Identity.Application.DTOs;
using DOL.SharedKernel;
using MediatR;

namespace DOL.Identity.Application.Commands.RefreshToken;

public record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken,
    string ClientIp = "127.0.0.1"
) : IRequest<Result<TokenResponseDto>>;
