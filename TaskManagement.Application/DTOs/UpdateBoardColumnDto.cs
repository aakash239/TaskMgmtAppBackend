namespace TaskManagement.Application.DTOs;

public record UpdateBoardColumnDto
(
    string Name,
    string? Description
);