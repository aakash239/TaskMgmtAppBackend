using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IAuthService
{
    Task<AuthenticationResultDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<AuthenticationResultDto> SignUpAsync(UserSignUpDto request, CancellationToken cancellationToken = default);
    Task<AuthenticationResultDto?> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken = default);
    Task LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default);
    Task LogoutAllAsync(int userId, CancellationToken cancellationToken = default);
}
