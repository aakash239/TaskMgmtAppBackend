namespace TaskManagement.Application.DTOs;

public record CreateBoardColumnDto
(
    string Name,
    string? Description
);