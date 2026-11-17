using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IUserRepository
{
    Task<UserCredentialsDto?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    // Inserts a user and returns the saved user 
    Task<CreatedUserDto> CreateAsync(UserInsertDto user, CancellationToken cancellationToken = default);
}
