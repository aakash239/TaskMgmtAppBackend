using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IAuthService
{
    Task<AuthSessionResultDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<AuthSessionResultDto> SignUpAsync(SignUpRequestDto request, CancellationToken cancellationToken = default);
    Task<AuthSessionResultDto?> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken = default);
    Task LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default);
    Task LogoutAllAsync(int userId, CancellationToken cancellationToken = default);
}
