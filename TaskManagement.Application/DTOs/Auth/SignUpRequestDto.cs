using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

// data that we recieve from frontend to create user
public record SignUpRequestDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(256, ErrorMessage = "Name cannot exceed 256 characters.")]
     public string Name {get; set;} = string.Empty;

     [Required(ErrorMessage = "Email is required.")]
     [EmailAddress(ErrorMessage = "Enter a valid email address.")]
     [StringLength(256, ErrorMessage = "Password is required.")]
     public string Email {get; set;} = string.Empty;

     [Required(ErrorMessage = "Password is Required")]
     public string Password{get;set;} = string.Empty;
    
}
