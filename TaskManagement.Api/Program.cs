using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Authentication;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Connection String
var connectionString = builder.Configuration.GetConnectionString("defaultConnection")??throw new InvalidOperationException("DB Connection string not configured properly");

// Services here
// Database
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Password Hashing and verification
builder.Services.AddScoped<IPasswordService, PasswordSerice>();


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
