using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public record UpdateProjectTemplateDto(
    [property: Required]
    [property: Range(1, int.MaxValue)]
    int TemplateId
);