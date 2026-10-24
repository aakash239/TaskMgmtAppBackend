using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenData?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task CreateSessionAsync(int userId, Guid sessionId, string tokenHash, DateTime expiresAt, CancellationToken cancellationToken = default);
    Task <bool> RotateTokenAsync(long refreshTokenId, string newTokenHash, DateTime expiresAt, CancellationToken cancellationToken = default);
    Task <bool> RevokeSessionsAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<int> RevokeAllSessionsAsync(int userId, CancellationToken cancellationToken = default);
}