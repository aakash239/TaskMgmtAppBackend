namespace TaskManagement.Application.DTOs;

// A safe representation of a user that is returned by the repository.
// PasswordHash.Deliberately Excluded.
public class UserDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}