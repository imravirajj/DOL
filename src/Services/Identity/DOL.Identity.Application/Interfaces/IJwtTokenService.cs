using DOL.Identity.Domain.Entities;

namespace DOL.Identity.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles);
    string GenerateRefreshToken();
    Guid? ValidateAccessToken(string token);
}
