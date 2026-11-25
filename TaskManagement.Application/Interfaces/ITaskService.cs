using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface ITaskService
{
    Task<List<TaskDto>> GetMyTasksAsync(
        int projectId,
        CancellationToken cancellationToken = default);

    Task<List<TaskDto>> GetAllProjectTasksAsync(
        int projectId,
        CancellationToken cancellationToken = default);

    Task<TaskDto> GetByIdAsync(
        int projectId,
        int taskId,
        CancellationToken cancellationToken = default);

    Task<TaskDto> CreateAsync(
        int projectId,
        CreateTaskDto request,
        CancellationToken cancellationToken = default);

    Task<TaskDto> UpdateAsync(
        int projectId,
        int taskId,
        UpdateTaskDto request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int projectId,
        int taskId,
        CancellationToken cancellationToken = default);
}