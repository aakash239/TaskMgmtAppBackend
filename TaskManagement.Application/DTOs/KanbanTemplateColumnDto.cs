namespace TaskManagement.Application.DTOs;

public record KanbanTemplateColumnDto(
    int TemplateColumnId,
    string Name,
    string? Description,
    int Position
);