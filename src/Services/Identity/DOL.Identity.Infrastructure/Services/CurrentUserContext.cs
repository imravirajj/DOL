using System.Security.Claims;
using DOL.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace DOL.Identity.Infrastructure.Services;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User?.FindFirst("sub")?.Value;
            return Guid.TryParse(idClaim, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value ?? User?.FindFirst("email")?.Value;

    public Guid? CompanyId
    {
        get
        {
            var tenantClaim = User?.FindFirst("tenant_id")?.Value
                ?? _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            return Guid.TryParse(tenantClaim, out var id) ? id : null;
        }
    }

    public Guid? BranchId
    {
        get
        {
            var branchClaim = User?.FindFirst("branch_id")?.Value
                ?? _httpContextAccessor.HttpContext?.Request.Headers["X-Branch-Id"].FirstOrDefault();
            return Guid.TryParse(branchClaim, out var id) ? id : null;
        }
    }

    public string? AccessScope => User?.FindFirst("access_scope")?.Value;

    public Guid? ScopeEntityId
    {
        get
        {
            var scopeClaim = User?.FindFirst("scope_entity_id")?.Value;
            return Guid.TryParse(scopeClaim, out var id) ? id : null;
        }
    }

    public IReadOnlyList<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public bool IsGlobalAdmin => Roles.Contains("Admin");

    public bool IsCompanyAdmin => IsGlobalAdmin || Roles.Contains("CompanyAdmin") || AccessScope == "CompanyLevel";
}
