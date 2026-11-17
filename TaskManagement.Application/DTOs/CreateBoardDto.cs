namespace TaskManagement.Application.DTOs;

public record CreateBoardDto( // userId will be supplied via claims so users cannot give arbitrary userId's
    int TemplateId,
    string Name,
    string? Description
);