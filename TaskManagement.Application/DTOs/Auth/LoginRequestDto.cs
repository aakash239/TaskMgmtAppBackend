using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public record LoginRequestDto
{
    [Required(ErrorMessage = "Email is Required")]
    [EmailAddress(ErrorMessage = "Enter a Valid email address")]
    [StringLength(256, ErrorMessage = "Email canniot exceed 256 characters")]
    public string Email {get; set;} = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password{get; set;} = string.Empty;
}
