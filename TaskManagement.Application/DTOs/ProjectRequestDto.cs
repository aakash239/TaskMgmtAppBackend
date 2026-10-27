using System.ComponentModel.DataAnnotations;
using TaskManagement.Core.Enums;

namespace TaskManagement.Application.DTOs;

public class ProjectRequestDto
{
   [Required(ErrorMessage = "Project Name is Required. ")]
   [StringLength(200, ErrorMessage = "Project name can be at most 200 characters.")]
   public string Name {get; set;} = string.Empty;

   [StringLength(2000, ErrorMessage = "Description can be at most 2000 characters.")]
   public string? Description {get; set;}

   [EnumDataType(typeof(ProjectStatus), ErrorMessage = "Status must be Active, InProgress or Completed.")]   
   public ProjectStatus Status {get; set;} = ProjectStatus.Active;

   public DateOnly? DueDate {get; set;}

}