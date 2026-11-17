namespace TaskManagement.Application.DTOs;

public record BoardColumnDto(
    int BoardColumnId,
    string Name,
    string? Description,
    int Position
);