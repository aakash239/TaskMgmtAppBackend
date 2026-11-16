namespace TaskManagement.Application.DTOs;

public record CreateKanbanTemplateDto( // userId will be supplied via claims so users cannot give arbitrary userId's
    string Name,
    string? Description,
    List<CreateTemplateColumnDto> Columns
);

public record CreateTemplateColumnDto(
    string Name,
    string? Description,
    int Position
);