using System.ComponentModel.DataAnnotations;
using TaskManagement.Core.Enums;

namespace TaskManagement.Application.DTOs;

public class UpdateTaskDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [EnumDataType(typeof(TaskPriority))]
    public TaskPriority Priority { get; set; }

    public DateOnly? DueDate { get; set; }

    [Range(1, int.MaxValue)]
    public int BoardColumnId { get; set; } // we need to move it's column to update a task 
}