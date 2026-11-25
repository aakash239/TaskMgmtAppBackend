using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public record UpdateBoardColumnDto
(
    [Required(ErrorMessage = "Column name is required.")]
    [StringLength(100, ErrorMessage = "Column name cannot exceed 100 characters.")]
    string Name,

    [StringLength(500, ErrorMessage = "Column description cannot exceed 500 characters.")]
    string? Description
);