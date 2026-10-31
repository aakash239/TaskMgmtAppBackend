using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Api.Middleware;
using TaskManagement.Application;
using TaskManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Check access tokens on protected endpoints.
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
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
                        ),
                        ClockSkew = TimeSpan.Zero // ensures expiratin is enforced without additional tolerance
                    };
                });
                
const string AngularCorsPolicy = "Angular";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
if (allowedOrigins == null)
{
    if (builder.Environment.IsDevelopment())
    {
        allowedOrigins = new[] { "http://localhost:4200" };
    }
    else
    {
        allowedOrigins = Array.Empty<string>();
    }
}

foreach (var origin in allowedOrigins)
{
    if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
    {
        throw new InvalidOperationException("Invalid CORS origin.");
    }

    bool isExactOrigin = origin == uri.GetLeftPart(UriPartial.Authority);
    bool isHttps = uri.Scheme == Uri.UriSchemeHttps;
    bool isLocalDevelopment = builder.Environment.IsDevelopment()
        && uri.Scheme == Uri.UriSchemeHttp && uri.Host == "localhost";

    if (!isExactOrigin || (!isHttps && !isLocalDevelopment))
    {
        throw new InvalidOperationException("CORS origins must be exact HTTPS origins; HTTP localhost is allowed only in Development.");
    }
}
builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularCorsPolicy, policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        }
    });
});
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}
app.UseCors(AngularCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
