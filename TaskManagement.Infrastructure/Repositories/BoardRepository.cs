using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Entities;

namespace TaskManagement.Infrastructure.Repositories;

public class BoardRepository (AppDbContext context) : IBoardRepository
{
    private readonly AppDbContext _context = context;


public async Task<BoardDto> GetOrCreateMyBoardAsync(
    int projectId,
    int userId,
    CancellationToken cancellationToken = default)
{
    // 1. Verify that the user belongs to this project.
    var isMember = await _context.ProjectMembers
        .AnyAsync(
            pm => pm.ProjectId == projectId && pm.UserId == userId,
            cancellationToken);

    if (!isMember)
        throw new KeyNotFoundException("Project not found or user is not a member.");

    // 2. Check whether the user already has a board.
    var existingBoard = await _context.Boards
        .AsNoTracking()
        .Where(b =>
            b.ProjectId == projectId &&
            b.UserId == userId &&
            !b.IsDeleted)
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
        ))
        .FirstOrDefaultAsync(cancellationToken);

    if (existingBoard != null)
        return existingBoard;

    // 3. Get the project's currently assigned template.
    var project = await _context.Projects
        .AsNoTracking()
        .Where(p => p.ProjectId == projectId)
        .Select(p => new { p.TemplateId })
        .FirstOrDefaultAsync(cancellationToken);

    if (project == null)
        throw new KeyNotFoundException("Project not found.");

    // 4. Load the template and its columns.
    var template = await _context.KanbanTemplates
        .AsNoTracking()
        .Include(t => t.Columns)
        .FirstOrDefaultAsync(
            t => t.TemplateId == project.TemplateId && !t.IsDeleted,
            cancellationToken);

    if (template == null)
        throw new KeyNotFoundException("Project template not found.");

    // 5. Create the board using the project's template.
    var now = DateTime.UtcNow;

    var board = new Board
    {
        ProjectId = projectId,
        UserId = userId,
        TemplateId = template.TemplateId,
        Name = "My Board",
        Description = null,
        CreatedAt = now,
        UpdatedAt = now,
        Columns = template.Columns
            .OrderBy(c => c.Position)
            .Select(c => new BoardColumn
            {
                Name = c.Name,
                Description = c.Description,
                Position = c.Position,
                CreatedAt = now,
                UpdatedAt = now
            })
            .ToList()
    };

    _context.Boards.Add(board);
    await _context.SaveChangesAsync(cancellationToken);

    // 6. Return the newly created board.
    return new BoardDto(
        board.BoardId,
        board.TemplateId,
        board.Name,
        board.Description,
        board.CreatedAt,
        board.UpdatedAt,
        board.Columns
            .OrderBy(c => c.Position)
            .Select(c => new BoardColumnDto(
                c.BoardColumnId,
                c.Name,
                c.Description,
                c.Position
            ))
            .ToList()
    );
}

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
    public async Task<int> GetOrCreateMyBoardAsync(int userId, CreateBoardDto dto, CancellationToken cancellationToken = default)
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

        board.IsDeleted = true;
        board.DeletedAt = DateTime.UtcNow;
        board.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    } 

    // Column Methods
    public async Task<IEnumerable<BoardColumnDto>> GetBoardColumnsAsync(int boardId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.BoardColumns
                                .AsNoTracking()
                                .Where(c => c.BoardId == boardId && c.Board.UserId == userId)
                                .OrderBy(c => c.Position)
                                .Select(c => new BoardColumnDto(
                                    c.BoardColumnId,
                                    c.Name,
                                    c.Description,
                                    c.Position
                                )).ToListAsync(cancellationToken);
    }
    public async Task<int> AddBoardColumnAsync(int boardId, int userId, CreateBoardColumnDto dto, CancellationToken cancellationToken = default)
    {
        var board = await _context.Boards
                                    .FirstOrDefaultAsync(b => b.BoardId == boardId && b.UserId == userId, cancellationToken);
        if(board == null)
            throw new KeyNotFoundException("Board not found");

        var maxPosition = await _context.BoardColumns
                                            .Where(c => c.Board.UserId == userId && c.BoardId == boardId)
                                            // .Select(c => (int?)c.Position)
                                            .CountAsync(cancellationToken);

        var column = new BoardColumn
        {
            BoardId = boardId,
            Name = dto.Name.Trim(),
            Description = dto.Description,
            Position = maxPosition + 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.BoardColumns.Add(column);
        await _context.SaveChangesAsync(cancellationToken);
        return column.BoardColumnId;
    }
    public async Task<bool> UpdateBoardColumnAsync(int boardId, int columnId, int userId, UpdateBoardColumnDto dto, CancellationToken cancellationToken = default)
    {
        var column = await _context.BoardColumns
                                        .FirstOrDefaultAsync(c => c.Board.UserId == userId && c.BoardId == boardId && c.BoardColumnId == columnId, cancellationToken);
        
        if(column == null) return false;

        column.Name = dto.Name.Trim();
        column.Description = dto.Description;
        column.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<bool> DeleteBoardColumnAsync(int boardId, int columnId, int userId, CancellationToken cancellationToken = default)
    {
        var column = await _context.BoardColumns
                                        .FirstOrDefaultAsync(c => c.BoardColumnId == columnId && c.Board.BoardId == boardId && c.Board.UserId == userId, cancellationToken);

        if(column == null) return false;

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _context.BoardColumns.Remove(column);
        await _context.SaveChangesAsync(cancellationToken);

        var remainingColumns = await _context.BoardColumns
                                                .Where(c => c.BoardId == boardId)
                                                .OrderBy(c => c.Position)
                                                .ToListAsync();
        // temporary move positions out of range ( max + 1 )
        // no problem in directly changing the number as it should not create a problem
        // but we do not know how db will implement the queries and some errors can occur 
        // example 1 2 3 _ 5 6 (4 is deleted) if 6 is renamed to 5 first b4 5 is renamed to 4(in db execution not us we always update in order) then error can occur
        var maxPosition = remainingColumns
                                .Select(c => c.Position)
                                .DefaultIfEmpty(0)
                                .Max(); 

        for(int i = 0; i < remainingColumns.Count; i++)
        {
            remainingColumns[i].Position = maxPosition + i + 1;
        }

        await _context.SaveChangesAsync(cancellationToken);
        
        // assign final sequential positions
        for(int i = 0; i < remainingColumns.Count; i++)
        {
            remainingColumns[i].Position = i + 1;
            remainingColumns[i].UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }
    public async Task<bool> ReorderBoardColumnsAsync(int boardId, int userId, ReorderColumnsDto dto, CancellationToken cancellationToken = default)
    {
        var boardExists = await _context.Boards
                                            .AnyAsync(b => b.BoardId == boardId && b.UserId == userId, cancellationToken);

        if(!boardExists)    return false;

        var columns = await _context.BoardColumns
                                        .Where(c => c.BoardId == boardId)
                                        .ToListAsync(cancellationToken);


        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var maxPosition = columns.Select(c => c.Position)
                                    .DefaultIfEmpty(0)
                                    .Max();     

        // move every column to temporary position ( > Max position )
        for(int i = 0; i < columns.Count; i++)
        {
            columns[i].Position = maxPosition + i + 1;
        }

        await _context.SaveChangesAsync(cancellationToken);

        //stage 2: Assign the requested positions: into a dict and then assign them as requested
        var columnLookup = columns.ToDictionary(c => c.BoardColumnId); // use hashmap instead of list for o(N) lookup

        for(int i = 0; i < dto.ColumnIds.Count; i++)
        {
            var columnId = dto.ColumnIds[i];
            columnLookup[columnId].Position = i + 1;
            columnLookup[columnId].UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}