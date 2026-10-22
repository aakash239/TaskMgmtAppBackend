namespace TaskManagement.Application.DTOs;

public record LoginUserDto(
    int UserId,
    string Name,
    string Email,
    string PasswordHash,
    bool IsActive
);