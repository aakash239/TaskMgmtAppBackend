namespace TaskManagement.Application.DTOs;

public record AccessTokenDto(
    string AccessToken,
    DateTime ExpiresAtUtc
);