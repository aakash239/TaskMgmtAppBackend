using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public record ReorderColumnsDto(
    [Required(ErrorMessage = "Column Ids are required")]
    [MinLength(1, ErrorMessage = " At least one cloumn ID must be provided")]
    List<int> ColumnIds     // use columnIds to represent position [4, 3, 1, 2] means 4th orignal is in 1st new position now...
);