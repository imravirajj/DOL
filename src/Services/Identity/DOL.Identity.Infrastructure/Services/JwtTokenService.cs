using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DOL.Identity.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles)
    {
        var secretKey = _configuration["Jwt:SecretKey"] ?? "SUPER_SECRET_KEY_FOR_DOL_PLATFORM_IDENTITY_SERVICE_NET10!";
        var issuer = _configuration["Jwt:Issuer"] ?? "DOL.Identity";
        var audience = _configuration["Jwt:Audience"] ?? "DOL.Platform";
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user.CompanyId.HasValue)
        {
            claims.Add(new Claim("tenant_id", user.CompanyId.Value.ToString()));
        }

        if (user.BranchId.HasValue)
        {
            claims.Add(new Claim("branch_id", user.BranchId.Value.ToString()));
        }

        claims.Add(new Claim("access_scope", user.Scope.ToString()));

        if (user.ScopeEntityId.HasValue)
        {
            claims.Add(new Claim("scope_entity_id", user.ScopeEntityId.Value.ToString()));
        }

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public Guid? ValidateAccessToken(string token)
    {
        var secretKey = _configuration["Jwt:SecretKey"] ?? "SUPER_SECRET_KEY_FOR_DOL_PLATFORM_IDENTITY_SERVICE_NET10!";
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false // Ignore lifetime here so we can validate expired access tokens for refresh token flow
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? principal.FindFirst("sub")?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
        }
        catch
        {
            return null;
        }

        return null;
    }
}
