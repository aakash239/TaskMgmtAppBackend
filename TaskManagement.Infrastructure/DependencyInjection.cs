using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Configuration;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Infrastructure.Security;

namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    // Called from Program.cs: builder.Services.AddInfrastructure(builder.Configuration)
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        // Bind the "Jwt" section of appsettings.json to JwtSettings,
        // so classes can ask for IOptions<JwtSettings>.
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();         // one per request (depends on the scoped DbContext)
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>(); // stateless, one shared instance
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>(); // stateless, one shared instance

        return services;
    }
}