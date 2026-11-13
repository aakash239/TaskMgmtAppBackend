namespace TaskManagement.Application.DTOs;

public record CreateKanbanTemplateDto(
    int UserId,
    string Name,
    string? Description,
    List<CreateTemplateColumnDto> Columns
);

public record CreateTemplateColumnDto(
    string Name,
    string? Description,
    int Position
);