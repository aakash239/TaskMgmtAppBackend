namespace TaskManagement.Application.DTOs;

// Data the service hands to the repository to create a new user.

public record UserInsertDto
(
    string Name,
    string Email,
    string NormalizedEmail,
    string PasswordHash
);