using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IUserRepository
{
    Task<LoginUserDto?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    // True if a user with this normalized email already exists.
    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    // Inserts a user and returns the saved user 
    Task<UserDto> CreateAsync(CreateUserDto user, CancellationToken cancellationToken = default);
}