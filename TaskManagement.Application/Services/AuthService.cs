using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Services;

public class AuthService (IUserRepository userRepository, IPasswordService passwordService, IJwtTokenService jwtTokenService): IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        // Validate Credentials
        if(string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }   

        // Normalize Email
        string normalizedEmail = request.Email.Trim().ToUpperInvariant();

        //retrieve user
        var user = await _userRepository.GetByNormalizedEmailAsync(normalizedEmail, cancellationToken);

        // check user existence and status
        if(user is null || !user.IsActive)
        {
            return null;
        }

        // Verify password
        bool isPasswordValid = _passwordService.VerifyPassword(user.PasswordHash, request.Password);
        if(!isPasswordValid) return null;

        // generate JWT
        var token = _jwtTokenService.GenerateToken(user.UserId, user.Email);

        return new LoginResponseDto
        {
            AccessToken = token.AccessToken,
            ExpiresAtUtc = token.ExpiresAtUtc
        };
    }
}