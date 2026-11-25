using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public record CreateKanbanTemplateDto( // userId will be supplied via claims so users cannot give arbitrary userId's

    [Required(ErrorMessage = " Template name is required.")]
    [StringLength(100, ErrorMessage = "Template name cannot exceed 100 characters.")]
    string Name,

    [StringLength(500, ErrorMessage = "Template description cannot exceed 500 characters.")]
    string? Description,

    [Required(ErrorMessage = " Atleast one template column is required")]
    List<CreateTemplateColumnDto> Columns
);

public record CreateTemplateColumnDto(
    [Required(ErrorMessage = "Column name is required")]
    [StringLength(100, ErrorMessage = "Column name cannot exceed 100 characters.")]
    string Name,

    [StringLength(500, ErrorMessage = "Column description cannot exceed 500 characters.")]
    string? Description,

    [Range(1, int.MaxValue, ErrorMessage = "Column position must be greater than zero.")]
    int Position
);