using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Constants;

namespace TaskManagement.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordService passwordService,
    IJwtTokenService jwtTokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IRefreshTokenGenerator refreshTokenGenerator) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;

    public async Task<AuthSessionResultDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
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

        var jwtUser = new JwtClaimsDto(user.UserId, user.Name, user.Email);
        return await CreateSessionAsync(jwtUser, cancellationToken);
    }

    public async Task<AuthSessionResultDto> SignUpAsync(SignUpRequestDto request, CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        var email = request.Email.Trim();
        var normalizedEmail = email.ToUpperInvariant();

        var passwordHash = _passwordService.HashPassword(request.Password);
        var newUser = new UserInsertDto(name, email, normalizedEmail, passwordHash);
        var user = await _userRepository.CreateAsync(newUser, cancellationToken);

        var jwtUser = new JwtClaimsDto(user.UserId, user.Name, user.Email);
        return await CreateSessionAsync(jwtUser, cancellationToken);
    }

    public async Task<AuthSessionResultDto?> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken = default)
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
        var jwtUser = new JwtClaimsDto(data.UserId, data.Name, data.Email);
        var accessToken = _jwtTokenService.GenerateAccessToken(jwtUser);

        var rotated = await _refreshTokenRepository.RotateTokenAsync(
            data.RefreshTokenId, replacementHash, expiresAt, cancellationToken);
        if (!rotated)
        {
            return null;
        }

        var response = new AuthResponseDto(
            jwtUser.UserId, jwtUser.Name, jwtUser.Email, accessToken.AccessToken, accessToken.ExpiresAtUtc);
        return new AuthSessionResultDto(response, replacement, expiresAt);
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

    public Task LogoutAllAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _refreshTokenRepository.RevokeAllSessionsAsync(userId, cancellationToken);
    }

    private async Task<RefreshTokenLookupDto?> FindTokenAsync(string? token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token) || token.Length > 256)
        {
            return null;
        }

        var tokenHash = _refreshTokenGenerator.HashRefreshToken(token);
        return await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);
    }

    private async Task<AuthSessionResultDto> CreateSessionAsync(JwtClaimsDto user, CancellationToken cancellationToken)
    {
        var token = _refreshTokenGenerator.GenerateRefreshToken();
        var tokenHash = _refreshTokenGenerator.HashRefreshToken(token);
        var now = DateTime.UtcNow;
        var expiresAt = now.AddDays(AuthConstants.RefreshSessionLifetimeDays);
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var sessionId = Guid.NewGuid();

        await _refreshTokenRepository.CreateSessionAsync(
            user.UserId, sessionId, tokenHash, expiresAt, AuthConstants.MaximumActiveSessions, cancellationToken);

        var response = new AuthResponseDto(
            user.UserId, user.Name, user.Email, accessToken.AccessToken, accessToken.ExpiresAtUtc);
        return new AuthSessionResultDto(response, token, expiresAt);
    }
}
