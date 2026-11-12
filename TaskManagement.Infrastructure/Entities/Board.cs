namespace TaskManagement.Infrastructure.Entities;

public class Board
{
    public int BoardId {get; set;}
    public int UserId {get; set;}
    public int TemplateId {get; set;}
    public string Name {get; set;} = string.Empty;
    public string? Description {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}

    // Navigation Properties
    public User User {get; set;} = null!;
    public KanbanTemplate Template {get; set;} = null!;
    public ICollection<BoardColumn> Columns = [];
}