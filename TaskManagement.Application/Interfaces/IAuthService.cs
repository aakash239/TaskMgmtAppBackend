using TaskManagement.Application.Models;

namespace TaskManagement.Application.Interfaces;

// Contract for authentication use-cases. The controller depends on this.
public interface IAuthService
{
    Task<AuthResponseDto> SignUpAsync(UserSignUpDto request, CancellationToken cancellationToken = default);
}