using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Exceptions;

namespace TaskManagement.Application.Services;

public class AuthService (IUserRepository userRepository, IPasswordService passwordService, IJwtTokenService jwtTokenService): IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
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
        var token = _jwtTokenService.GenerateAccessToken(new JwtCreationDto(user.UserId, user.Name, user.Email));

        return new AuthResponseDto
        (
            user.UserId,
            user.Name,
            user.Email,
            token.AccessToken,
            token.ExpiresAtUtc
        );
    }

    public async Task<TokenRefreshResponseDto?> RefreshTokenAsync(TokenRefereshDto request, CancellationToken cancellationToken = default)
    {
        
    }
    public async Task<AuthResponseDto> SignUpAsync(UserSignUpDto request, CancellationToken cancellationToken = default)
    {
        // Cleaning the input
        var name = request.Name.Trim();
        var email = request.Email.Trim();

        
        var normalizedEmail = email.ToUpperInvariant();

        //  error if the email is already registered.
        if (await _userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new ConflictException("An account with this email already exists.");
        }

        // Hash the password. We never store or log the plain-text password.
        var passwordHash = _passwordService.HashPassword(request.Password);

        // Save the user. 
        var createdUser = await _userRepository.CreateAsync(new CreateUserDto
        (
            name,
            email,
            normalizedEmail,
            passwordHash
        ), cancellationToken);

        //  JWT so the user is signed in immediately after sign-up.
        var accessToken = _jwtTokenService.GenerateAccessToken(new JwtCreationDto(createdUser.UserId, createdUser.Name, createdUser.Email));

        // Shape the response for the client.
        return new AuthResponseDto
        (
            createdUser.UserId,
            createdUser.Name,
            createdUser.Email,
            accessToken.AccessToken,
            accessToken.ExpiresAtUtc
        );
    }
}