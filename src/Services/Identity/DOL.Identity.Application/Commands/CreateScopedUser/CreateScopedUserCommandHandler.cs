using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.CreateScopedUser;

public class CreateScopedUserCommandHandler : IRequestHandler<CreateScopedUserCommand, Result<UserDto>>
{
    private readonly IIdentityDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IPasswordHasher _passwordHasher;

    public CreateScopedUserCommandHandler(
        IIdentityDbContext context,
        ICurrentUserContext currentUserContext,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _currentUserContext = currentUserContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserDto>> Handle(CreateScopedUserCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserContext.CompanyId.HasValue)
        {
            return Result.Failure<UserDto>("Tenant context is missing. Cannot create scoped user.");
        }

        var companyId = _currentUserContext.CompanyId.Value;

        // Security check: If caller is Branch-scoped, they can only create users in their own branch
        if (!_currentUserContext.IsCompanyAdmin && _currentUserContext.AccessScope == AccessScope.BranchLevel.ToString())
        {
            if (request.BranchId != _currentUserContext.BranchId)
            {
                return Result.Failure<UserDto>("Access denied. You can only create users for your assigned branch.");
            }

            if (request.Role == ApplicationRole.CompanyAdmin)
            {
                return Result.Failure<UserDto>("Access denied. Branch staff cannot grant CompanyAdmin privileges.");
            }
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var userExists = await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
        if (userExists)
        {
            return Result.Failure<UserDto>("User with this email already exists.");
        }

        // Validate branch if provided
        if (request.BranchId.HasValue)
        {
            var branchExists = await _context.Branches
                .AnyAsync(b => b.Id == request.BranchId.Value && b.CompanyId == companyId, cancellationToken);

            if (!branchExists)
            {
                return Result.Failure<UserDto>("Specified branch was not found in this company.");
            }
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var targetScope = request.Scope;
        var targetScopeId = request.ScopeEntityId ?? request.BranchId;

        var user = new ApplicationUser(
            request.FirstName,
            request.LastName,
            email,
            request.PhoneNumber,
            passwordHash,
            companyId,
            targetScope,
            targetScopeId,
            request.BranchId
        );

        _context.Users.Add(user);

        // Assign role
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.Role, cancellationToken);
        if (role == null)
        {
            return Result.Failure<UserDto>($"Role '{request.Role}' does not exist.");
        }

        _context.UserRoles.Add(new Domain.Entities.UserRole(user.Id, role.Id));
        await _context.SaveChangesAsync(cancellationToken);

        var rolesList = new List<string> { role.Name };
        var dto = new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber,
            user.Status.ToString(),
            user.EmailConfirmed,
            rolesList,
            user.CreatedAt,
            user.CompanyId,
            user.BranchId,
            user.Scope.ToString(),
            user.ScopeEntityId
        );

        return Result.Success(dto);
    }
}
