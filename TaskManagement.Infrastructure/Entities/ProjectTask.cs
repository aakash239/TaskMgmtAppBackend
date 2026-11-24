using TaskManagement.Core.Enums;

namespace TaskManagement.Infrastructure.Entities;

public class ProjectTask
{
    public int TaskId { get; set; }

    public int BoardId { get; set; }
    public int BoardColumnId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateOnly? DueDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Board Board { get; set; } = null!;
    public BoardColumn BoardColumn { get; set; } = null!;
}