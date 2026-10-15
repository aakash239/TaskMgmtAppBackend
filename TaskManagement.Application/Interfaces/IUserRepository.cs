using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IUserRepository
{
    Task<LoginUserDto?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);
}