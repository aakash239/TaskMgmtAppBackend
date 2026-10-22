using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IAuthService
{
    // returns a successful loginresponseDTo if login is successful else returns null.
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);  

    Task<AuthResponseDto> SignUpAsync(UserSignUpDto request, CancellationToken cancellationToken = default);
}