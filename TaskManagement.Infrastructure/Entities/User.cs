namespace TaskManagement.Infrastructure.Entities;

public class User
{
    public string Name {get; set;} = string.Empty;
    public int UserId {get; set;}
    public string Email {get; set;} = string.Empty;
    public string NormalizedEmail {get; set;} = string.Empty;
    public string PasswordHash {get; set;} = string.Empty;
    public bool IsActive {get; set;} = true;
    public DateTime CreatedAt{get; set;} = DateTime.UtcNow;

    // navigation properties
    public ICollection<UserSession> Sessions {get; set;} = []; // List of current user sessions
    public ICollection<KanbanTemplate> KanbanTemplates {get; set;} = []; // list of Kanban Templates Created by the user
    public ICollection<Board> Boards {get; set;} = []; // List of Boards Created by the User

}



