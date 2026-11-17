using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Entities;

namespace TaskManagement.Infrastructure.Repositories;

public class BoardRepository (AppDbContext context) : IBoardRepository
{
    private readonly AppDbContext _context = context;
    public async Task<IEnumerable<BoardDto>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Boards
                            .AsNoTracking()
                            .Where(b => b.UserId == userId)
                            .Select(b => new BoardDto(
                                b.BoardId,
                                b.TemplateId,
                                b.Name,
                                b.Description,
                                b.CreatedAt,
                                b.UpdatedAt,
                                b.Columns
                                    .OrderBy(c => c.Position)
                                    .Select(c => new BoardColumnDto(
                                        c.BoardColumnId,
                                        c.Name,
                                        c.Description,
                                        c.Position
                                    )).ToList()
                            )).ToListAsync(cancellationToken);
    }
    public async Task<BoardDto?> GetByIdAsync(int boardId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Boards
                            .AsNoTracking()
                            .Where(b => b.UserId == userId && b.BoardId == boardId)
                            .Select(b => new BoardDto(
                                b.BoardId,
                                b.TemplateId,
                                b.Name,
                                b.Description,
                                b.CreatedAt,
                                b.UpdatedAt,
                                b.Columns
                                    .OrderBy(c => c.Position)
                                    .Select(c => new BoardColumnDto(
                                        c.BoardColumnId,
                                        c.Name,
                                        c.Description,
                                        c.Position
                                    )).ToList()
                            )).FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<int> CreateBoardAsync(int userId, CreateBoardDto dto, CancellationToken cancellationToken = default)
    {
        var template = await _context.KanbanTemplates
                                    .AsNoTracking()
                                    .Include(t => t.Columns)
                                    .FirstOrDefaultAsync(t => t.TemplateId == dto.TemplateId && t.UserId == userId, cancellationToken);

        if (template == null)
            throw new KeyNotFoundException("Kanban template Not found");
        
        var board = new Board
        {
            UserId = userId,
            TemplateId = template.TemplateId,
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Columns = template.Columns
                            .OrderBy(c => c.Position)
                            .Select(c => new BoardColumn
                            {
                                Name = c.Name,
                                Description = c.Description,
                                Position = c.Position,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                            }).ToList()
        };
        _context.Boards.Add(board);
        await _context.SaveChangesAsync(cancellationToken);
        return board.BoardId;
    }
    public async Task<bool> DeleteBoardByIdAsync(int boardId, int userId, CancellationToken cancellationToken = default)
    {
        var board = await _context.Boards
                                .FirstOrDefaultAsync(b => b.BoardId == boardId && b.UserId == userId, cancellationToken);

        if(board == null)
            return false;

        _context.Boards.Remove(board);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    } 
}