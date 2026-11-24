using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public class UpdateProjectTemplateDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int TemplateId {get; set;}
}