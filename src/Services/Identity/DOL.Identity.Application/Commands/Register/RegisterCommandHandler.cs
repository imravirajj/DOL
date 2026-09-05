using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResultDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterCommandHandler(
        IIdentityDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResultDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users
            .AnyAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (existingUser)
        {
            return Result.Failure<AuthResultDto>("User with this email already exists.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = new ApplicationUser(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            passwordHash
        );

        _context.Users.Add(user);

        // Assign role
        var roleName = string.IsNullOrWhiteSpace(request.Role) ? ApplicationRole.Buyer : request.Role;
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        if (role != null)
        {
            _context.UserRoles.Add(new Domain.Entities.UserRole(user.Id, role.Id));
        }

        // Generate tokens
        var roles = new List<string> { roleName };
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        _context.RefreshTokens.Add(new Domain.Entities.RefreshToken(user.Id, refreshTokenValue, refreshTokenExpiry, "127.0.0.1"));

        await _context.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber,
            user.Status.ToString(),
            user.EmailConfirmed,
            roles,
            user.CreatedAt
        );

        var tokenResponse = new TokenResponseDto(
            accessToken,
            refreshTokenValue,
            refreshTokenExpiry
        );

        return Result.Success(new AuthResultDto(userDto, tokenResponse));
    }
}
