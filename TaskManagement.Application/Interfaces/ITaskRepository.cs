using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface ITaskRepository
{
    Task<List<TaskDto>> GetMyTasksAsync(
        int projectId,
        int userId,
        CancellationToken cancellationToken = default);

    Task<List<TaskDto>> GetAllProjectTasksAsync(
        int projectId,
        CancellationToken cancellationToken = default);

    Task<TaskDto?> GetByIdAsync(
        int taskId,
        int projectId,
        int userId,
        CancellationToken cancellationToken = default);

    Task<TaskDto> CreateAsync(
        int projectId,
        int userId,
        CreateTaskDto request,
        CancellationToken cancellationToken = default);

    Task<TaskDto?> UpdateAsync(
        int taskId,
        int projectId,
        int userId,
        UpdateTaskDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int taskId,
        int projectId,
        int userId,
        CancellationToken cancellationToken = default);
}