using TaskManagement.Core.Enums;

namespace TaskManagement.Application.DTOs;
// for a project in the trash
public record TrashedProjectDto
{
     public int ProjectId { get; set; }

     public string Name { get; set; } = string.Empty;

     public ProjectStatus Status { get; set; }

     public DateTime DeletedAt { get; set; }
}