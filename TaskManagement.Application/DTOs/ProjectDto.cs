using TaskManagement.Core.Enums;
namespace TaskManagement.Application.DTOs;

public class ProjectDto
{
    public int ProjectId {get; set;}
    public string Name {get; set;} = string.Empty;
    public string? Description {get; set;}
    public ProjectStatus Status {get; set;}

    public DateOnly? DueDate {get; set;}

    public DateTime CreatedAt {get; set;}

    public DateTime? UpdatedAt {get; set;}

    public ProjectRole MyRole {get; set;}


}
