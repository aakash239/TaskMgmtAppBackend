namespace TaskManagement.Application.DTOs;

public record KanbanTemplateDto(
    int TemplateId,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<KanbanTemplateColumnDto> Columns
);