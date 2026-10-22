namespace TaskManagement.Application.DTOs;

public record JwtCreationDto
(
    int UserId,
    string Name,
    string Email
);