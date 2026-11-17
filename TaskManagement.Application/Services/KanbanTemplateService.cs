using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Services;

public class KanbanTemplateService(IKanbanTemplateRepositiory templateRepositiory): IKanbanTemplateService
{
    private readonly IKanbanTemplateRepositiory _templateRepository = templateRepositiory;
    public Task<IEnumerable<KanbanTemplateDto>> GetAllAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _templateRepository.GetAllByUserIdAsync(userId, cancellationToken);
    }
    public Task<KanbanTemplateDto?> GetByIdAsync(int templateId, int userId, CancellationToken cancellationToken = default)
    {
        return _templateRepository.GetByIdAsync(templateId, userId, cancellationToken);
    }
    public Task<int> CreateTemplateAsync(int userId, CreateKanbanTemplateDto dto, CancellationToken cancellationToken = default)
    {
        ValidateTemplate(dto);
        return _templateRepository.CreateKanbanTemplateAsync(userId, dto, cancellationToken);
    }
    public Task<bool> DeleteAsync(int templateId, int userId, CancellationToken cancellationToken = default)
    {
        return _templateRepository.DeleteKanbanTemplateByIdAsync(templateId, userId, cancellationToken);
    }

    public static void ValidateTemplate(CreateKanbanTemplateDto dto)
    {
        if(string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Template name is required!");
        
        if(dto.Columns == null || dto.Columns.Count == 0) throw new ArgumentException("A template must contain alteast one column!");

        if(dto.Columns.Any(c => string.IsNullOrWhiteSpace(c.Name))) throw new ArgumentException("Column name cannot be empty!");

        if(dto.Columns.Any(c => c.Position < 1)) throw new ArgumentException("Column positon cannot be less than 1");

        if(dto.Columns.Select(c => c.Position).Distinct().Count() != dto.Columns.Count) throw new ArgumentException("Column positions Must be unique");
    }
}

