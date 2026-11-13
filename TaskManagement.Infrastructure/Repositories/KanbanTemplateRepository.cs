using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public class KanbanTemplateRepository(AppDbContext context) : IKanbanTemplateRepositiory{
    private readonly AppDbContext _context = context;
    public async Task<IEnumerable<KanbanTemplateDto>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default){
        return await _context.KanbanTemplates
                            .AsNoTracking()
                            .Where(t => t.UserId == userId)
                            .OrderBy(t => t.Name)
                            .Select(t => new KanbanTemplateDto(
                                t.TemplateId,
                                t.Name,
                                t.Description,
                                t.CreatedAt,
                                t.UpdatedAt,
                                t.Columns
                                    .OrderBy(c => c.Position)
                                    .Select(c => new KanbanTemplateColumnDto(
                                        c.TemplateColumnId,
                                        c.Name,
                                        c.Description,
                                        c.Position
                                    )).ToList()
                            )).ToListAsync();
    }
}