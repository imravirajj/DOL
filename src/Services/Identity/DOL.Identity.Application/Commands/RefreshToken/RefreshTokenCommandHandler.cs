using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResponseDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(
        IIdentityDbContext context,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<TokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = _jwtTokenService.ValidateAccessToken(request.AccessToken);
        if (userId == null)
        {
            return Result.Failure<TokenResponseDto>("Invalid or expired access token.");
        }

        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

        if (user == null)
        {
            return Result.Failure<TokenResponseDto>("User not found.");
        }

        // Query refresh token directly from DbContext to avoid EF tracking issues
        var existingRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && t.UserId == userId.Value, cancellationToken);

        if (existingRefreshToken == null || !existingRefreshToken.IsActive)
        {
            return Result.Failure<TokenResponseDto>("Invalid or expired refresh token.");
        }

        var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();
        var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        // Revoke old token
        existingRefreshToken.Revoke(newRefreshTokenValue);

        // Add new token directly via DbContext
        var newRefreshToken = new Domain.Entities.RefreshToken(user.Id, newRefreshTokenValue, newRefreshTokenExpiry, request.ClientIp);
        _context.RefreshTokens.Add(newRefreshToken);

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user, roles);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new TokenResponseDto(
            newAccessToken,
            newRefreshTokenValue,
            newRefreshTokenExpiry
        ));
    }
}
