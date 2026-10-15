namespace TaskManagement.Infrastructure.Entities;

public class User
{
    public string Name {get; set;} = string.Empty;
    public int UserId {get; set;}
    public string Email {get; set;} = string.Empty;
    public string NormalizedEmail {get; set;} = string.Empty;
    public string PasswordHash {get; set;} = string.Empty;
    public bool IsActive {get; set;}
    public DateTime CreatedAt{get; set;}
}