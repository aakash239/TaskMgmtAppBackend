using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public record UpdateProjectTemplateDto(
    [Range(1, int.MaxValue, ErrorMessage = "A valid template must be selected.")]
    int TemplateId
);