namespace TaskManagement.Application.DTOs;

public record JwtTokenDto(
    string AccessToken,
    DateTime ExpiresAtUtc
);