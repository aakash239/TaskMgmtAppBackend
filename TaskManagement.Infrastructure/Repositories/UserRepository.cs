using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Models;
using TaskManagement.Core.Exceptions;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Entities;

namespace TaskManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
       
        return _context.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        // DTO -> entity.
        var entity = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            NormalizedEmail = dto.NormalizedEmail,
            PasswordHash = dto.PasswordHash,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(entity); 

        await _context.SaveChangesAsync(cancellationToken);
        
       
        // Entity -> DTO (so PasswordHash never leaves this layer).
        return new UserDto
        {
            UserId = entity.UserId,
            Name = entity.Name,
            Email = entity.Email,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }
}