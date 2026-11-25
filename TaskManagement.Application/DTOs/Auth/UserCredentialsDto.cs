namespace TaskManagement.Application.DTOs;

public record UserCredentialsDto(
    int UserId,
    string Name,
    string Email,
    string PasswordHash,
    bool IsActive
);