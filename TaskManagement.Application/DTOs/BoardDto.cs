namespace TaskManagement.Application.DTOs;

public record BoardDto(
    int BoardId,
    int TemplateId,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<BoardColumnDto> Columns
);