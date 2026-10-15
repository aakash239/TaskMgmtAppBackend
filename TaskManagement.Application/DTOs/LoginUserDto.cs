namespace TaskManagement.Application.DTOs;

public record LoginUserDto(
    int UserId,
    string Email,
    string PasswordHash,
    bool IsActive
);