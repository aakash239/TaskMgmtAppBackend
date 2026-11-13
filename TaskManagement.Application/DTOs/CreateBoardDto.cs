namespace TaskManagement.Application.DTOs;

public record CreateBoardDto(
    int UserId,
    int TemplateId,
    string Name,
    string? Description
);