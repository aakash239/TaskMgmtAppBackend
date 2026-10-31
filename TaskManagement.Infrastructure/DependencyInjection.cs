using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Configuration;
using TaskManagement.Infrastructure.Authentication;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Infrastructure.Security;

namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Load and check the JWT settings before the API starts.
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .Validate(settings =>
                !string.IsNullOrWhiteSpace(settings.Issuer) &&
                !string.IsNullOrWhiteSpace(settings.Audience) &&
                !string.IsNullOrWhiteSpace(settings.Key) &&
                Encoding.UTF8.GetByteCount(settings.Key) >= 32 &&
                settings.AccessTokenExpiryMinutes == 15,
                "JWT issuer, audience, signing key, and 15-minute expiry must be configured.")
            .ValidateOnStart();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();

        var connectionString = configuration.GetConnectionString("defaultConnection");
        if (connectionString == null)
        {
            throw new InvalidOperationException("DB Connection string not configured properly");
        }

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }
}
