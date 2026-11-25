using TaskManagement.Core.Enums;

namespace TaskManagement.Application.DTOs;

public class TaskDto
{
    public int TaskId { get; set; }

    public int BoardId { get; set; }
    public int BoardColumnId { get; set; }

    public string ColumnName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public TaskPriority Priority { get; set; }

    public DateOnly? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Populated when the admin fetches all project tasks.
    public int? OwnerUserId { get; set; }
}