using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.DTOs;
using TaskManagement.Infrastructure.Data;

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
                                u.Email,
                                u.PasswordHash,
                                u.IsActive
                            )).FirstOrDefaultAsync(cancellationToken);
    }

}