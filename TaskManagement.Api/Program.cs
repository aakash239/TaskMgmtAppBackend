using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Application.Interfaces;
using TaskManagement.Api.Middleware;
using TaskManagement.Application;
using TaskManagement.Infrastructure;
using TaskManagement.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);  


// ---------- Application & Infrastructure services ----------
builder.Services.AddApplication();                          // AuthService
builder.Services.AddInfrastructure(builder.Configuration);  // repository, hasher, JWT generator, JwtSettings binding

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
                    options.MapInboundClaims = false; // preserves the original JWT Claim names: sub, email, name, jti
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
   // what to do here...? 
}

app.UseHttpsRedirection();
app.UseCors(AngularCorsPolicy); 
app.UseAuthentication();    // reads and validates the Bearer token; must come BEFORE UseAuthorization
app.UseAuthorization();     // enforces [Authorize]

app.MapControllers();

app.Run();