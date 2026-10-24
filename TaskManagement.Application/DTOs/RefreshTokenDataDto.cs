namespace TaskManagement.Application.DTOs;

public sealed record RefreshTokenData(
    long RefreshTokenId,
    Guid SessionId,
    int UserId,
    DateTime SessionExpiresAt,
    DateTime TokenExpiresAt,
    DateTime? SessionRevokedAt,
    DateTime? TokenRevokedAt,
    long? ReplacedByTokenId,
    bool IsActiveUser
);