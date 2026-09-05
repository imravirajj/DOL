using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.Identity.Domain.Events;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.RegisterCompany;

public class RegisterCompanyCommandHandler : IRequestHandler<RegisterCompanyCommand, Result<AuthResultDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterCompanyCommandHandler(
        IIdentityDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResultDto>> Handle(RegisterCompanyCommand request, CancellationToken cancellationToken)
    {
        var normalizedCode = request.CompanyCode.Trim().ToUpperInvariant();
        var companyExists = await _context.Companies
            .AnyAsync(c => c.Code == normalizedCode || c.Email == request.CompanyEmail.ToLowerInvariant(), cancellationToken);

        if (companyExists)
        {
            return Result.Failure<AuthResultDto>("Company with this code or email already exists.");
        }

        var userExists = await _context.Users
            .AnyAsync(u => u.Email == request.AdminEmail.ToLowerInvariant(), cancellationToken);

        if (userExists)
        {
            return Result.Failure<AuthResultDto>("User with this email already exists.");
        }

        // 1. Create Company (Tenant)
        var company = new Company(
            request.CompanyName,
            normalizedCode,
            request.CompanyEmail,
            request.CompanyPhone,
            request.CompanyAddress,
            "Enterprise",
            request.Currency,
            request.TimeZone
        );
        _context.Companies.Add(company);

        // 2. Setup Initial Geographic Hierarchy: Country -> State -> City
        var country = new Country(company.Id, request.CountryName, request.CountryIsoCode);
        _context.Countries.Add(country);

        var state = new StateRegion(company.Id, country.Id, request.StateName);
        _context.StateRegions.Add(state);

        var city = new City(company.Id, state.Id, request.CityName);
        _context.Cities.Add(city);

        // 3. Create Default Headquarters Branch
        var mainBranch = new Branch(
            company.Id,
            city.Id,
            request.MainBranchName,
            request.MainBranchCode,
            request.CompanyAddress ?? $"{request.CityName} HQ",
            request.CompanyPhone,
            request.CompanyEmail,
            isMainBranch: true
        );
        _context.Branches.Add(mainBranch);

        // 4. Create Company Super Admin User
        var passwordHash = _passwordHasher.HashPassword(request.AdminPassword);
        var adminUser = new ApplicationUser(
            request.AdminFirstName,
            request.AdminLastName,
            request.AdminEmail,
            request.AdminPhoneNumber,
            passwordHash,
            company.Id,
            AccessScope.CompanyLevel,
            company.Id,
            mainBranch.Id
        );
        adminUser.ConfirmEmail();
        _context.Users.Add(adminUser);

        // 5. Assign CompanyAdmin Role
        var companyAdminRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == ApplicationRole.CompanyAdmin, cancellationToken);

        if (companyAdminRole != null)
        {
            _context.UserRoles.Add(new Domain.Entities.UserRole(adminUser.Id, companyAdminRole.Id));
        }

        // 6. Raise Domain Event
        adminUser.AddDomainEvent(new CompanyRegisteredEvent(
            company.Id,
            company.Name,
            company.Code,
            adminUser.Id,
            adminUser.Email
        ));

        // 7. Generate JWT and Refresh Token
        var roles = new List<string> { ApplicationRole.CompanyAdmin };
        var accessToken = _jwtTokenService.GenerateAccessToken(adminUser, roles);
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        _context.RefreshTokens.Add(new Domain.Entities.RefreshToken(
            adminUser.Id,
            refreshTokenValue,
            refreshTokenExpiry,
            "127.0.0.1"
        ));

        await _context.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto(
            adminUser.Id,
            adminUser.FirstName,
            adminUser.LastName,
            adminUser.Email,
            adminUser.PhoneNumber,
            adminUser.Status.ToString(),
            adminUser.EmailConfirmed,
            roles,
            adminUser.CreatedAt,
            company.Id,
            mainBranch.Id,
            adminUser.Scope.ToString(),
            company.Id
        );

        var tokenResponse = new TokenResponseDto(
            accessToken,
            refreshTokenValue,
            refreshTokenExpiry
        );

        return Result.Success(new AuthResultDto(userDto, tokenResponse));
    }
}
