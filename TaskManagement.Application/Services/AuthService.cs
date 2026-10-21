using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Models;
using TaskManagement.Core.Exceptions;

namespace TaskManagement.Application.Services;

public class AuthService (IUserRepository userRepository, IPasswordService passwordService, IJwtTokenService jwtTokenService, IPasswordHasher passwordHasher): IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;          // hashing abstraction
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
        var passwordHash = _passwordHasher.Hash(request.Password);

        // Save the user. 
        var createdUser = await _userRepository.CreateAsync(new CreateUserDto
        {
            Name = name,
            Email = email,
            NormalizedEmail = normalizedEmail,
            PasswordHash = passwordHash
        }, cancellationToken);

        //  JWT so the user is signed in immediately after sign-up.
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(createdUser);

        // Shape the response for the client.
        return new AuthResponseDto
        {
            UserId = createdUser.UserId,
            Name = createdUser.Name,
            Email = createdUser.Email,
            AccessToken = accessToken.Token,
            ExpiresAtUtc = accessToken.ExpiresAtUtc
        };
    }
}