namespace TaskManagement.Application.DTOs;

// used internally between Application and API
// Extract RefreshToken put it in HttpOnly cookie and return access token info to angular
public sealed record AuthenticationResultDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt
);