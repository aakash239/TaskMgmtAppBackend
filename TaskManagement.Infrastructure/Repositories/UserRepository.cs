using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.DTOs;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Entities;

namespace TaskManagement.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task<LoginUserDto?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return await _context.Users
                            .AsNoTracking()
                            .Where(u => u.NormalizedEmail == normalizedEmail)
                            .Select(u => new LoginUserDto(
                                u.UserId,
                                u.Name,
                                u.Email,
                                u.PasswordHash,
                                u.IsActive
                            )).FirstOrDefaultAsync(cancellationToken);
    }
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