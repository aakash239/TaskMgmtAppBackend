using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.DTOs;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Entities;
using TaskManagement.Core.Exceptions;

namespace TaskManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserCredentialsDto?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return await _context.Users
                            .AsNoTracking()
                            .Where(u => u.NormalizedEmail == normalizedEmail)
                            .Select(u => new UserCredentialsDto(
                                u.UserId,
                                u.Name,
                                u.Email,
                                u.PasswordHash,
                                u.IsActive
                            )).FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<CreatedUserDto> CreateAsync(UserInsertDto dto, CancellationToken cancellationToken = default)
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

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicateEmail(ex))
        {
            // Translate the normalized-email constraint into a signup conflict.
            _context.Entry(entity).State = EntityState.Detached;
            throw new ConflictException("An account with this email already exists.");
        }
        
       
        // Return public user data; the login projection separately includes the hash for verification.
        return new CreatedUserDto
        {
            UserId = entity.UserId,
            Name = entity.Name,
            Email = entity.Email,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }

    private static bool IsDuplicateEmail(DbUpdateException exception)
    {
        var sqlException = exception.InnerException as SqlException;
        if (sqlException == null)
        {
            return false;
        }

        foreach (SqlError error in sqlException.Errors)
        {
            bool isUniqueConstraintError = error.Number == 2601 || error.Number == 2627;
            bool isEmailConstraint = error.Message.Contains("UniqueNormalizedEmail", StringComparison.Ordinal);

            if (isUniqueConstraintError && isEmailConstraint)
            {
                return true;
            }
        }

        return false;
    }
}
