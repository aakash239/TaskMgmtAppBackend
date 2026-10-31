using System.ComponentModel.DataAnnotations;
namespace TaskManagement.Application.DTOs;

// data that we recieve from frontend to create user
public class SignUpRequestDto
{
    [Required]
    [StringLength(256)]
     public string Name {get; set;} = string.Empty;
     [Required]
     [EmailAddress]
     [StringLength(256)]
     public string Email {get; set;} = string.Empty;
     [Required]
     public string Password{get;set;} = string.Empty;
    
}
