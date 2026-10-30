using Microsoft.EntityFrameworkCore.Query;
using TaskManagement.Core.Enums;
namespace TaskManagement.Infrastructure.Entities;

public class Project
{
    public int ProjectId {get; set;}
    public string Name {get; set;} = string.Empty;
    public string? Description {get; set;}

    public ProjectStatus Status {get;set;} = ProjectStatus.Active;

    public DateOnly? DueDate {get; set;}

    public int CreatedByUserId {get; set;}

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public DateTime? UpdatedAt {get; set;}

    public bool IsDeleted {get;set;}

    public DateTime? DeletedAt {get; set;}

    public int? DeletedByUserId {get;set;}

    // all membership rows 
    public ICollection<ProjectMember> Members {get; set;} = new List<ProjectMember>();







}