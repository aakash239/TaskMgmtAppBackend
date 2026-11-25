namespace TaskManagement.Application.DTOs;

// References A board's column
public record BoardColumnDto(
    int BoardColumnId,
    string Name,
    string? Description,
    int Position
);