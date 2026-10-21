using TaskManagement.Application.DTOs;
using TaskManagement.Application.Models;

namespace TaskManagement.Application.Interfaces;

public interface IAuthService
{
    // returns a successful loginresponseDTo if login is successful else returns null.
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);  

    Task<AuthResponseDto> SignUpAsync(UserSignUpDto request, CancellationToken cancellationToken = default);
}