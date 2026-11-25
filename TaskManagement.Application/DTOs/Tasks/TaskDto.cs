using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;
public record TaskDto
{
    [Required(ErrorMessage = "Task Id is Required")]
    [Range(1, int.MaxValue, ErrorMessage = "Task Id out of Range")]
    public int TaskId { get; set; }

    [Required(ErrorMessage = "Board Id is Required")]
    [Range(1, int.MaxValue, ErrorMessage = "Board Id out of Range")]
    public int BoardId { get; set; }

    [Required(ErrorMessage = "Board Column Id is Required")]
    [Range(1, int.MaxValue, ErrorMessage = "Board Column Id is Range")]
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