namespace TaskManagement.Application.Models;

// Data the service hands to the repository to insert a new user.

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;            
    public string NormalizedEmail { get; set; } = string.Empty;  
    public string PasswordHash { get; set; } = string.Empty;
}