using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IAuthService
{
    // returns a successful loginresponseDTo if login is successful else returns null.
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    Task<TokenRefreshResponseDto?> RefreshTokenAsync(TokenRefereshDto request, CancellationToken cancellationToken = default); 
    // returns a successful AuthResponseDto if signup is successful
    Task<AuthResponseDto> SignUpAsync(UserSignUpDto request, CancellationToken cancellationToken = default);
}