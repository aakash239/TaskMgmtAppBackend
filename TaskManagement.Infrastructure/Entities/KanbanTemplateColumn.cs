namespace TaskManagement.Infrastructure.Entities;

public class KanbanTemplateColumn
{
    public int TemplateColumnId {get; set;}
    public int TemplateId {get; set;}
    public string Name {get; set;} = string.Empty;
    public string? Description {get; set;}
    public int Position {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}

    // Navigation Properties
    public KanbanTemplate Template {get; set;} = null!;
}