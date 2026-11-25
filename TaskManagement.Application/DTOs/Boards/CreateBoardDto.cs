using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public record CreateBoardDto( // userId will be supplied via claims so users cannot give arbitrary userId's
    [Range(1, int.MaxValue, ErrorMessage = "A valid template must be selected.")]
    int TemplateId,

    [Required(ErrorMessage = "Board name is required.")]
    [StringLength(100, ErrorMessage = "Board name cannot exceed 100 characters.")]
    string Name,

    [StringLength(500, ErrorMessage = "Board description cannot exceed 500 characters.")]
    string? Description
);