namespace TaskManagement.Application.DTOs;

public record JwtClaimsDto
(
    int UserId,
    string Name,
    string Email
);