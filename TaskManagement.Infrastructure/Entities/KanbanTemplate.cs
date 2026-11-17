namespace TaskManagement.Infrastructure.Entities;

public class KanbanTemplate
{
    public int TemplateId {get; set;}
    public int UserId{get; set;}
    public string Name {get; set;} = string.Empty;
    public string? Description {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}

    //Navigation Properties
    public User User {get; set;} = null!;
    public ICollection<KanbanTemplateColumn> Columns {get; set;} = [];
    public ICollection<Board> Boards {get; set;} = [];
    
}