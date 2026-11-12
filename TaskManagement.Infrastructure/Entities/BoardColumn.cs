namespace TaskManagement.Infrastructure.Entities;

public class BoardColumn
{
    public int BoardColumnId {get; set;}
    public int BoardId {get; set;}
    public string Name {get; set;} = string.Empty;
    public string? Description {get; set;}
    public int Position {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
    
    //Navigation Properties
    public Board Board {get; set;} = null!;
}