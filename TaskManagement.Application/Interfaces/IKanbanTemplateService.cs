using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IKanbanTemplateService
{
    Task<IEnumerable<KanbanTemplateDto>> GetAllAsync(int userId, CancellationToken cancellationToken = default);
    Task<KanbanTemplateDto?> GetByIdAsync(int templateId, int userId, CancellationToken cancellationToken = default);
    Task<int> CreateTemplateAsync(int userId, CreateKanbanTemplateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int templateId, int userId, CancellationToken cancellationToken = default);
}