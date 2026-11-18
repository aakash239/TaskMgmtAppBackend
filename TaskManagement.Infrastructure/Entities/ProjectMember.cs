using TaskManagement.Core.Enums;

namespace TaskManagement.Infrastructure.Entities;

//  one row = one user's role in one project.
public class ProjectMember
{
public int ProjectId { get; set; }
public int UserId { get; set; }
public ProjectRole Role { get; set; }
public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

public Project Project { get; set; } = null!; // navigation to the project 
}

