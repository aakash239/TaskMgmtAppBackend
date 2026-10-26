using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;

namespace TaskManagement.Application;

public static class DependencyInjection
{
    // Extension method so Program.cs can call it
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Scoped = one instance per HTTP request.
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
