namespace TaskManagement.Application.DTOs;

public record AuthResponseDto
(
      int UserId,
      string Name,
      string Email,
      string AccessToken,
      DateTime ExpiresAtUtc
);