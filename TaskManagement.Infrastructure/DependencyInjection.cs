using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Configuration;
using TaskManagement.Infrastructure.Authentication;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Infrastructure.BackgroundJobs;
namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    // Called from Program.cs: builder.Services.AddInfrastructure(builder.Configuration)
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        // Bind the "Jwt" section of appsettings.json to JwtSettings,
        // so classes can ask for IOptions<JwtSettings>.
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // binding the "Projects" section so classes can ask for IOptions<ProjectSettings>
        services.Configure<ProjectSettings>(configuration.GetSection(ProjectSettings.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();         // one per request (depends on the scoped DbContext)
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IProjectRepository, ProjectRepository>();

        // Connection String Configuration
        var connectionString = configuration.GetConnectionString("defaultConnection") ?? throw new InvalidOperationException("DB Connection string not configured properly");

        // Services here
        // Database
        // Register EF Core's DbContext (scoped per request by default).
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("defaultConnection")));
        services.AddHostedService<TrashPurgeService>();
        return services;
    }
}