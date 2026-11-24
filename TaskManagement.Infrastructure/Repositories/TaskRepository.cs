using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Entities;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskRepository(AppDbContext context) : ITaskRepository
{
    private readonly AppDbContext _context = context;

    private static IQueryable<TaskDto> ProjectTaskQuery(
        IQueryable<ProjectTask> query)
    {
        return query.Select(t => new TaskDto
        {
            TaskId = t.TaskId,
            BoardId = t.BoardId,
            BoardColumnId = t.BoardColumnId,
            ColumnName = t.BoardColumn.Name,
            Title = t.Title,
            Description = t.Description,
            Priority = t.Priority,
            DueDate = t.DueDate,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            OwnerUserId = t.Board.UserId
        });
    }

    public async Task<List<TaskDto>> GetMyTasksAsync(
        int projectId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Tasks
            .AsNoTracking()
            .Where(t =>
                t.Board.ProjectId == projectId &&
                t.Board.UserId == userId &&
                !t.Board.IsDeleted);

        return await ProjectTaskQuery(query)
            .OrderBy(t => t.BoardColumnId)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskDto>> GetAllProjectTasksAsync(
        int projectId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Tasks
            .AsNoTracking()
            .Where(t =>
                t.Board.ProjectId == projectId &&
                !t.Board.IsDeleted);

        return await ProjectTaskQuery(query)
            .OrderBy(t => t.OwnerUserId)
            .ThenBy(t => t.BoardColumnId)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskDto?> GetByIdAsync(
        int taskId,
        int projectId,
        int? userId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Tasks
            .AsNoTracking()
            .Where(t =>
                t.TaskId == taskId &&
                t.Board.ProjectId == projectId &&
                !t.Board.IsDeleted);

        if (userId.HasValue)
        {
            query = query.Where(t => t.Board.UserId == userId.Value);
        }

        return await ProjectTaskQuery(query)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TaskDto> CreateAsync(
        int projectId,
        int userId,
        CreateTaskDto request,
        CancellationToken cancellationToken = default)
    {
        var board = await _context.Boards
            .FirstOrDefaultAsync(
                b => b.ProjectId == projectId &&
                     b.UserId == userId &&
                     !b.IsDeleted,
                cancellationToken);

        if (board == null)
        {
            throw new KeyNotFoundException(
                "Your board was not found for this project.");
        }

        var columnExists = await _context.BoardColumns
            .AnyAsync(
                c => c.BoardColumnId == request.BoardColumnId &&
                     c.BoardId == board.BoardId,
                cancellationToken);

        if (!columnExists)
        {
            throw new ArgumentException(
                "The selected column does not belong to your board.");
        }

        var now = DateTime.UtcNow;

        var task = new ProjectTask
        {
            BoardId = board.BoardId,
            BoardColumnId = request.BoardColumnId,
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),
            Priority = request.Priority,
            DueDate = request.DueDate,
            CreatedAt = now
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(
            task.TaskId,
            projectId,
            userId,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "The created task could not be retrieved.");
    }

    public async Task<TaskDto?> UpdateAsync(
        int taskId,
        int projectId,
        int userId,
        UpdateTaskDto request,
        CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(
                t => t.TaskId == taskId &&
                     t.Board.ProjectId == projectId &&
                     t.Board.UserId == userId &&
                     !t.Board.IsDeleted,
                cancellationToken);

        if (task == null)
        {
            return null;
        }

        var columnExists = await _context.BoardColumns
            .AnyAsync(
                c => c.BoardColumnId == request.BoardColumnId &&
                     c.BoardId == task.BoardId,
                cancellationToken);

        if (!columnExists)
        {
            throw new ArgumentException(
                "The selected column does not belong to your board.");
        }

        task.Title = request.Title.Trim();
        task.Description = string.IsNullOrWhiteSpace(request.Description)
            ? null
            : request.Description.Trim();
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.BoardColumnId = request.BoardColumnId;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(
            taskId,
            projectId,
            userId,
            cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        int taskId,
        int projectId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(
                t => t.TaskId == taskId &&
                     t.Board.ProjectId == projectId &&
                     t.Board.UserId == userId &&
                     !t.Board.IsDeleted,
                cancellationToken);

        if (task == null)
        {
            return false;
        }

        task.IsDeleted = true;
        task.DeletedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public Task<TaskDto?> GetByIdAsync(int taskId, int projectId, int userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
