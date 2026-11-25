using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public record CreateBoardColumnDto
(
    [Required(ErrorMessage = "Column name is required.")]
    [StringLength(100, ErrorMessage = "Column Name Cannot exceed 100 characters")]
    string Name,
    [StringLength(500, ErrorMessage = "Column Description Cannot exceed 500 characters")]
    string? Description
);