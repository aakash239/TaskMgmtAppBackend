using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Constants;
using TaskManagement.Core.Exceptions;

namespace TaskManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenGenerator refreshTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task<AuthenticationResultDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var normalizedEmail = request.Email.Trim().ToUpperInvariant();
        var user = await _userRepository.GetByNormalizedEmailAsync(normalizedEmail, cancellationToken);
        if (user == null || !user.IsActive)
        {
            return null;
        }

        var passwordMatches = _passwordService.VerifyPassword(user.PasswordHash, request.Password);
        if (!passwordMatches)
        {
            return null;
        }

        var jwtUser = new JwtCreationDto(user.UserId, user.Name, user.Email);
        return await CreateSessionAsync(jwtUser, cancellationToken);
    }

    public async Task<AuthenticationResultDto> SignUpAsync(UserSignUpDto request, CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        var email = request.Email.Trim();
        var normalizedEmail = email.ToUpperInvariant();

        var emailExists = await _userRepository.EmailExistsAsync(normalizedEmail, cancellationToken);
        if (emailExists)
        {
            throw new ConflictException("An account with this email already exists.");
        }

        var passwordHash = _passwordService.HashPassword(request.Password);
        var newUser = new CreateUserDto(name, email, normalizedEmail, passwordHash);
        var user = await _userRepository.CreateAsync(newUser, cancellationToken);

        var jwtUser = new JwtCreationDto(user.UserId, user.Name, user.Email);
        return await CreateSessionAsync(jwtUser, cancellationToken);
    }

    public async Task<AuthenticationResultDto?> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken = default)
    {
        var data = await FindTokenAsync(refreshToken, cancellationToken);
        var now = DateTime.UtcNow;
        if (data == null || !data.IsActiveUser)
        {
            return null;
        }

        if (data.SessionRevokedAt != null || data.TokenRevokedAt != null || data.ReplacedByTokenId != null)
        {
            return null;
        }

        if (data.SessionExpiresAt <= now || data.TokenExpiresAt <= now)
        {
            return null;
        }

        // Keep the original session expiry when replacing the token.
        var expiresAt = DateTime.SpecifyKind(data.SessionExpiresAt, DateTimeKind.Utc);
        var replacement = _refreshTokenGenerator.GenerateRefreshToken();
        var replacementHash = _refreshTokenGenerator.HashRefreshToken(replacement);
        var jwtUser = new JwtCreationDto(data.UserId, data.Name, data.Email);
        var accessToken = _jwtTokenService.GenerateAccessToken(jwtUser);

        var rotated = await _refreshTokenRepository.RotateTokenAsync(
            data.RefreshTokenId, replacementHash, expiresAt, cancellationToken);
        if (!rotated)
        {
            return null;
        }

        return CreateResult(jwtUser, accessToken, replacement, expiresAt);
    }

    public async Task LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default)
    {
        var data = await FindTokenAsync(refreshToken, cancellationToken);
        if (data == null)
        {
            return;
        }

        // An old rotated token should not log out the current session.
        if (data.TokenRevokedAt != null || data.ReplacedByTokenId != null || data.SessionRevokedAt != null)
        {
            return;
        }

        await _refreshTokenRepository.RevokeSessionsAsync(data.SessionId, cancellationToken);
    }

    public async Task LogoutAllAsync(int userId, CancellationToken cancellationToken = default)
    {
        await _refreshTokenRepository.RevokeAllSessionsAsync(userId, cancellationToken);
    }

    private async Task<RefreshTokenData?> FindTokenAsync(string? token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token) || token.Length > 256)
        {
            return null;
        }

        var tokenHash = _refreshTokenGenerator.HashRefreshToken(token);
        return await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);
    }

    private async Task<AuthenticationResultDto> CreateSessionAsync(JwtCreationDto user, CancellationToken cancellationToken)
    {
        var token = _refreshTokenGenerator.GenerateRefreshToken();
        var tokenHash = _refreshTokenGenerator.HashRefreshToken(token);
        var now = DateTime.UtcNow;
        var expiresAt = now.AddDays(AuthConstants.RefreshSessionLifetimeDays);
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var sessionId = Guid.NewGuid();

        await _refreshTokenRepository.CreateSessionAsync(
            user.UserId, sessionId, tokenHash, expiresAt, AuthConstants.MaximumActiveSessions, cancellationToken);

        return CreateResult(user, accessToken, token, expiresAt);
    }

    private static AuthenticationResultDto CreateResult(JwtCreationDto user, JwtTokenDto token, string refreshToken, DateTime refreshExpiresAt)
    {
        var response = new AuthResponseDto(user.UserId, user.Name, user.Email, token.AccessToken, token.ExpiresAtUtc);
        return new AuthenticationResultDto(response, refreshToken, refreshExpiresAt);
    }
}
