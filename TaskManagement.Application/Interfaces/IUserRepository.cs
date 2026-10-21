using TaskManagement.Application.Models;

namespace TaskManagement.Application.Interfaces;

// Contract for user data access. Implemented in Infrastructure.
public interface IUserRepository
{
    // True if a user with this normalized email already exists.
    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    // Inserts a user and returns the saved user 
    Task<UserDto> CreateAsync(CreateUserDto user, CancellationToken cancellationToken = default);
}