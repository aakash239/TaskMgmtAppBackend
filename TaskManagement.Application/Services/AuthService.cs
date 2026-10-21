using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Models;
using TaskManagement.Core.Exceptions;

namespace TaskManagement.Application.Services;

// Business logic for sign-up. Knows WHAT to do, not HOW the DB/JWT work.
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;          // data access abstraction
    private readonly IPasswordHasher _passwordHasher;          // hashing abstraction
    private readonly IJwtTokenGenerator _jwtTokenGenerator;    // token abstraction

    // Dependencies are injected by the DI container.
    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
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