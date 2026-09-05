using System.Text;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Infrastructure.Persistence;
using DOL.Identity.Infrastructure.Services;
using DOL.SharedKernel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DOL.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=dol;Username=postgres;Password=postgres";

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

        services.AddScoped<IIdentityDbContext>(provider => provider.GetRequiredService<IdentityDbContext>());

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailService, ConsoleEmailService>();

        // Configure JWT Authentication
        var secretKey = configuration["Jwt:SecretKey"] ?? "SUPER_SECRET_KEY_FOR_DOL_PLATFORM_IDENTITY_SERVICE_NET10!";
        var issuer = configuration["Jwt:Issuer"] ?? "DOL.Identity";
        var audience = configuration["Jwt:Audience"] ?? "DOL.Platform";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

        return services;
    }
}
