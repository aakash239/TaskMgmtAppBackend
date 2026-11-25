using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public record CreateTaskDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [EnumDataType(typeof(TaskPriority))]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateOnly? DueDate { get; set; }

    [Range(1, int.MaxValue)]
    public int BoardColumnId { get; set; }
}