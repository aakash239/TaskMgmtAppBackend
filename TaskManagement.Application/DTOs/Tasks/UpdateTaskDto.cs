using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;
public record UpdateTaskDto
{
    [Required(ErrorMessage = "Task title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Task title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Task Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [EnumDataType(typeof(TaskPriority), ErrorMessage = "Priority must be low, medium or high")]
    public TaskPriority Priority { get; set; }

    public DateOnly? DueDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A valid board column must be selected.")]
    public int BoardColumnId { get; set; } // we need to move it's column to update a task 
}