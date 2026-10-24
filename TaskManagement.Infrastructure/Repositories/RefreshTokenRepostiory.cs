using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Entities;

namespace TaskManagement.Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext context): IRefreshTokenRepository
{
    private readonly AppDbContext _context = context;

    public async Task<RefreshTokenData?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens.AsNoTracking()
                        .Where(rt => rt.TokenHash == tokenHash)
                        .Select(rt => new RefreshTokenData(
                            rt.RefreshTokenId,
                            rt.SessionId,
                            rt.Session.UserId,
                            rt.Session.ExpiresAt,
                            rt.ExpiresAt,
                            rt.Session.RevokedAt,
                            rt.RevokedAt,
                            rt.ReplacedByTokenId,
                            rt.Session.User.IsActive
                        )).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task CreateSessionAsync(int userId, Guid sessionId, string tokenHash, DateTime expiresAt, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var activeSessions = await _context.UserSessions
                                        .Where(s => s.UserId == userId && s.RevokedAt == null && s.ExpiresAt > now)
                                        .OrderBy(s => s.CreatedAt)
                                        .ThenBy(s => s.SessionId)
                                        .ToListAsync(cancellationToken);

        if (activeSessions.Count >= 3)
        {
            int sessionsToRevoke = activeSessions.Count - 2;
            foreach (var session in activeSessions.Take(sessionsToRevoke))
            {
                session.RevokedAt = now;
            }
        }

        var UserSession = new UserSession
        {
            SessionId = sessionId,
            UserId = userId,
            CreatedAt = now,
            ExpiresAt = expiresAt
        };

        var refreshToken = new RefreshToken
        {
            SessionId = sessionId,
            TokenHash = tokenHash,
            CreatedAt = now,
            ExpiresAt = expiresAt
        };

        _context.UserSessions.Add(UserSession);
        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RotateTokenAsync(long refreshTokenId, string newTokenHash, DateTime expiresAt, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var oldToken = await _context.RefreshTokens.Include(rt => rt.Session)
                                    .FirstOrDefaultAsync(rt => rt.RefreshTokenId == refreshTokenId, cancellationToken);

        if( oldToken == null || 
            oldToken.RevokedAt != null ||
            oldToken.ExpiresAt <= now ||
            oldToken.Session.RevokedAt != null ||
            oldToken.Session.ExpiresAt <= now)
        {
            return false;
        }

        var newToken = new RefreshToken
        {
            SessionId = oldToken.SessionId,
            TokenHash = newTokenHash,
            CreatedAt = now,
            ExpiresAt = expiresAt < oldToken.Session.ExpiresAt ? expiresAt : oldToken.Session.ExpiresAt
        };

        oldToken.RevokedAt = now;
        oldToken.ReplacedByToken = newToken;
        oldToken.Session.LastActivityAt = now;

        _context.RefreshTokens.Add(newToken);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RevokeSessionsAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _context.UserSessions
                                    .FirstOrDefaultAsync(s => s.SessionId == sessionId, cancellationToken);

        if(session == null || session.RevokedAt != null)
        {
            return false;
        }

        session.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> RevokeAllSessionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var activeSessions = await _context.UserSessions
                                        .Where(s => s.UserId == userId &&
                                        s.RevokedAt == null && s.ExpiresAt > now)
                                        .ToListAsync(cancellationToken);
        
        foreach(var session in activeSessions)
        {
            session.RevokedAt = now;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return activeSessions.Count;
    }
}