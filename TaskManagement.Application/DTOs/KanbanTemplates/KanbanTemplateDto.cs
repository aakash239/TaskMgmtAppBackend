using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public record KanbanTemplateDto(
    [Required]
    [Range(1, int.MaxValue)]
    int TemplateId,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<KanbanTemplateColumnDto> Columns
);