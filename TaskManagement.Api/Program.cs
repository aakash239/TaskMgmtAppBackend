using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Api.Middleware;
using TaskManagement.Application;
using TaskManagement.Core.Configuration;
using TaskManagement.Infrastructure;
using TaskManagement.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);  

// ---------- Database ----------

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DB Connection string not configured properly");

// Register EF Core's DbContext (scoped per request by default).
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// ---------- Application & Infrastructure services ----------
builder.Services.AddApplication();                          // AuthService
builder.Services.AddInfrastructure(builder.Configuration);  // repository, hasher, JWT generator, JwtSettings binding

// ---------- JWT authentication ----------
// The auth middleware needs the same settings the token generator uses, to VALIDATE incoming tokens.
var jwt = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt settings are missing.");

// Fail at startup if the secret was never set (user-secrets / environment variable).
if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
    throw new InvalidOperationException("Jwt:Key must be set and at least 32 characters long.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)   // use Bearer tokens by default
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;                        // keep claim names as "sub", "email", "name"
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                               // token's "iss" must match
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,                             // token's "aud" must match
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,                     // signature must be valid
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ValidateLifetime = true,                             // reject expired tokens
            ClockSkew = TimeSpan.FromMinutes(1),                 // small tolerance for clock differences
            NameClaimType = "name"                               // User.Identity.Name reads the "name" claim
        };
    });

builder.Services.AddAuthorization();                             // enables [Authorize]

// ---------- Framework services ----------
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// ---------- HTTP pipeline (order matters) ----------
app.UseMiddleware<ExceptionHandlingMiddleware>();   // first, so it catches errors from everything below

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();    // reads and validates the Bearer token; must come BEFORE UseAuthorization
app.UseAuthorization();     // enforces [Authorize]

app.MapControllers();

app.Run();