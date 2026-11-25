using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public record CreateTaskDto
{
    [Required(ErrorMessage = "Task title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Task title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Task description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [EnumDataType(typeof(TaskPriority), ErrorMessage = "Priority must be Low, Medium or High")]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateOnly? DueDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A vlsid board column must be selected.")]
    public int BoardColumnId { get; set; }
}