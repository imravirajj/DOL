using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResultDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IIdentityDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (user == null)
        {
            return Result.Failure<AuthResultDto>("Invalid email or password.");
        }

        if (user.IsLockedOut)
        {
            return Result.Failure<AuthResultDto>($"Account is locked out until {user.LockoutEnd:u}.");
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            user.RecordFailedLogin();
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Failure<AuthResultDto>("Invalid email or password.");
        }

        user.ResetFailedLogin();

        // Check company active status if user belongs to a company
        if (user.CompanyId.HasValue)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == user.CompanyId.Value, cancellationToken);

            if (company != null && company.Status != Domain.Enums.CompanyStatus.Active && company.Status != Domain.Enums.CompanyStatus.Trial)
            {
                return Result.Failure<AuthResultDto>($"Company account is {company.Status}. Please contact support.");
            }
        }

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        if (!roles.Any())
        {
            roles.Add("Buyer");
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        // Add refresh token directly via DbContext to avoid EF tracking issues
        var refreshToken = new Domain.Entities.RefreshToken(user.Id, refreshTokenValue, refreshTokenExpiry, request.ClientIp);
        _context.RefreshTokens.Add(refreshToken);

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
            user.CreatedAt,
            user.CompanyId,
            user.BranchId,
            user.Scope.ToString(),
            user.ScopeEntityId
        );

        var tokenResponse = new TokenResponseDto(
            accessToken,
            refreshTokenValue,
            refreshTokenExpiry
        );

        return Result.Success(new AuthResultDto(userDto, tokenResponse));
    }
}

