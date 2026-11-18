using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Entities;

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
                            )).ToListAsync(cancellationToken);
    }

    public async Task<KanbanTemplateDto?> GetByIdAsync(int templateId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.KanbanTemplates
                                .AsNoTracking()
                                .Where(t => t.TemplateId == templateId && t.UserId == userId)
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
                                )).FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<int> CreateKanbanTemplateAsync(int userId, CreateKanbanTemplateDto dto, CancellationToken cancellationToken = default)
    {
        var template = new KanbanTemplate
        {
            UserId = userId,
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Columns = [.. dto.Columns.Select(c => new KanbanTemplateColumn
                        {
                            Name = c.Name,
                            Description = c.Description,
                            Position = c.Position,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        })]
        };

        _context.KanbanTemplates.Add(template);
        await _context.SaveChangesAsync(cancellationToken);
        return template.TemplateId;
    }
    public async Task<bool> DeleteKanbanTemplateByIdAsync(int templateId, int userId, CancellationToken cancellationToken = default)
    {
        var template = await _context.KanbanTemplates.FirstOrDefaultAsync(
            t => t.TemplateId == templateId && t.UserId == userId, cancellationToken
        );

        if (template == null)
            return false;
        
        template.IsDeleted = true;
        template.DeletedAt = DateTime.UtcNow;
        template.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    } 
}