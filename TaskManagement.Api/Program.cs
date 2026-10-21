using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;
using TaskManagement.Infrastructure.Authentication;
using TaskManagement.Api.Middleware;
using TaskManagement.Application;
using TaskManagement.Core.Configuration;
using TaskManagement.Infrastructure;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);  

// Connection String Configuration
var connectionString = builder.Configuration.GetConnectionString("defaultConnection")??throw new InvalidOperationException("DB Connection string not configured properly");

// JWT Configuration
builder.Services.Configure<JwtOptions> (builder.Configuration.GetSection("Jwt"));

// Services here
// Database
// Register EF Core's DbContext (scoped per request by default).
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Password Hashing and verification
builder.Services.AddScoped<IPasswordService, PasswordSerice>();

// JWT
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

//Auth Service
builder.Services.AddScoped<IAuthService, AuthService>();


// ---------- Application & Infrastructure services ----------
builder.Services.AddApplication();                          // AuthService
builder.Services.AddInfrastructure(builder.Configuration);  // repository, hasher, JWT generator, JwtSettings binding

// ---------- JWT authentication ----------
// The auth middleware needs the same settings the token generator uses, to VALIDATE incoming tokens.
// var jwt = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
//     ?? throw new InvalidOperationException("Jwt settings are missing.");

// // Fail at startup if the secret was never set (user-secrets / environment variable).
// if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
//     throw new InvalidOperationException("Jwt:Key must be set and at least 32 characters long.");

// builder.Services
//     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)   // use Bearer tokens by default
//     .AddJwtBearer(options =>
//     {
//         options.MapInboundClaims = false;                        // keep claim names as "sub", "email", "name"
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,                               // token's "iss" must match
//             ValidIssuer = jwt.Issuer,
//             ValidateAudience = true,                             // token's "aud" must match
//             ValidAudience = jwt.Audience,
//             ValidateIssuerSigningKey = true,                     // signature must be valid
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
//             ValidateLifetime = true,                             // reject expired tokens
//             ClockSkew = TimeSpan.FromMinutes(1),                 // small tolerance for clock differences
//             NameClaimType = "name"                               // User.Identity.Name reads the "name" claim
//         };
//     });

builder.Services.AddAuthorization();                             // enables [Authorize]

// ---------- Framework services ----------
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
// ---------- CORS ----------
const string AngularCorsPolicy = "AngularDev";            // name of the policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:4200")        // only the Angular dev server may call the API
              .AllowAnyHeader()                            // allows Content-Type and Authorization headers
              .AllowAnyMethod());                          
});



var app = builder.Build();

// ---------- HTTP pipeline (order matters) ----------
app.UseMiddleware<ExceptionHandlingMiddleware>();   // first, so it catches errors from everything below

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
app.UseCors(AngularCorsPolicy); 
app.UseAuthentication();    // reads and validates the Bearer token; must come BEFORE UseAuthorization
app.UseAuthorization();     // enforces [Authorize]

app.MapControllers();

app.Run();