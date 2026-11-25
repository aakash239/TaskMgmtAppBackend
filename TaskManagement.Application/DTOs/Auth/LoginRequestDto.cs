using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public record LoginRequestDto
{
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email {get; set;} = string.Empty;

    [Required]
    public string Password{get; set;} = string.Empty;
}
