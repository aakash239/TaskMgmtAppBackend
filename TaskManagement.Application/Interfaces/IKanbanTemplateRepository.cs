using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IKanbanTemplateRepositiory
{
    Task<IEnumerable<KanbanTemplateDto>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<KanbanTemplateDto?> GetByIdAsync(int templateId, int userId, CancellationToken cancellationToken = default);
    Task<int> CreateKanbanTemplateAsync(int userId, CreateKanbanTemplateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteKanbanTemplateByIdAsync(int templateId, int userId, CancellationToken cancellationToken = default); 
}