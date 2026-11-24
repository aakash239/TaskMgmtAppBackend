namespace TaskManagement.Application.DTOs;

public record ReorderColumnsDto(
    List<int> ColumnIds     // use columnIds to represent position [4, 3, 1, 2] means 4th orignal is in 1st position now...
);