using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;
using TaskManagement.Infrastructure.Authentication;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Connection String Configuration
var connectionString = builder.Configuration.GetConnectionString("defaultConnection")??throw new InvalidOperationException("DB Connection string not configured properly");

// JWT Configuration
builder.Services.Configure<JwtOptions> (builder.Configuration.GetSection("Jwt"));

// Services here
// Database
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Password Hashing and verification
builder.Services.AddScoped<IPasswordService, PasswordSerice>();

// JWT
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

//Auth Service
builder.Services.AddScoped<IAuthService, AuthService>();


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// jwt Validation
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                jwt["Key"] ?? throw new InvalidOperationException("JWT signing key is missing")
                            )
                        )
                    };
                });
                
builder.Services.AddAuthorization();


var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    var passwordService = scope.ServiceProvider
        .GetRequiredService<IPasswordService>();

    string email = "test@example.com";
    string password = "TestPassword123!";

    string normalizedEmail = email.Trim().ToUpperInvariant();

    bool userExists = await context.Users
        .AnyAsync(u => u.NormalizedEmail == normalizedEmail);

    if (!userExists)
    {
        var user = new TaskManagement.Infrastructure.Entities.User
{
    Name = "Test User",
    Email = email,
    NormalizedEmail = normalizedEmail,
    PasswordHash = passwordService.HashPassword(password),
    IsActive = true,
    CreatedAt = DateTime.UtcNow
};

        context.Users.Add(user);

        await context.SaveChangesAsync();

        Console.WriteLine("Development test user created.");
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
