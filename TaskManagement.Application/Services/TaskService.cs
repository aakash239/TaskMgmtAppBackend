using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Enums;
using TaskManagement.Core.Exceptions;

namespace TaskManagement.Application.Services;

public class TaskService(
    ITaskRepository taskRepository,
    IProjectRepository projectRepository,
    ICurrentUserService currentUser) : ITaskService
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task<List<TaskDto>> GetMyTasksAsync(
        int projectId,
        CancellationToken cancellationToken = default)
    {
        await RequireProjectMemberAsync(projectId, cancellationToken);

        return await _taskRepository.GetMyTasksAsync(
            projectId,
            _currentUser.UserId,
            cancellationToken);
    }

    public async Task<List<TaskDto>> GetAllProjectTasksAsync(
        int projectId,
        CancellationToken cancellationToken = default)
    {
        await RequireProjectAdminAsync(projectId, cancellationToken);

        return await _taskRepository.GetAllProjectTasksAsync(
            projectId,
            cancellationToken);
    }

    public async Task<TaskDto> GetByIdAsync(
        int projectId,
        int taskId,
        CancellationToken cancellationToken = default)
    {
        var role = await RequireProjectMemberAsync(
            projectId,
            cancellationToken);

        var userId = role == ProjectRole.Admin
            ? (int?)null
            : _currentUser.UserId;

        var task = await _taskRepository.GetByIdAsync(
            taskId,
            projectId,
            userId,
            cancellationToken);

        return task ?? throw new NotFoundException("Task not found.");
    }

    public async Task<TaskDto> CreateAsync(
        int projectId,
        CreateTaskDto request,
        CancellationToken cancellationToken = default)
    {
        await RequireProjectMemberAsync(projectId, cancellationToken);

        Normalize(request);

        try
        {
            return await _taskRepository.CreateAsync(
                projectId,
                _currentUser.UserId,
                request,
                cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            throw new NotFoundException(
                "Your board was not found for this project.");
        }
    }

    public async Task<TaskDto> UpdateAsync(
        int projectId,
        int taskId,
        UpdateTaskDto request,
        CancellationToken cancellationToken = default)
    {
        await RequireProjectMemberAsync(projectId, cancellationToken);

        Normalize(request);

        var updated = await _taskRepository.UpdateAsync(
            taskId,
            projectId,
            _currentUser.UserId,
            request,
            cancellationToken);

        return updated ?? throw new NotFoundException(
            "Task not found or you do not own this task.");
    }

    public async Task DeleteAsync(
        int projectId,
        int taskId,
        CancellationToken cancellationToken = default)
    {
        await RequireProjectMemberAsync(projectId, cancellationToken);

        var deleted = await _taskRepository.DeleteAsync(
            taskId,
            projectId,
            _currentUser.UserId,
            cancellationToken);

        if (!deleted)
        {
            throw new NotFoundException(
                "Task not found or you do not own this task.");
        }
    }

    private async Task<ProjectRole> RequireProjectMemberAsync(
        int projectId,
        CancellationToken cancellationToken)
    {
        var role = await _projectRepository.GetUserRoleAsync(
            projectId,
            _currentUser.UserId,
            includeDeleted: false,
            cancellationToken);

        if (role == null)
        {
            throw new NotFoundException("Project not found.");
        }

        return role.Value;
    }

    private async Task RequireProjectAdminAsync(
        int projectId,
        CancellationToken cancellationToken)
    {
        var role = await RequireProjectMemberAsync(
            projectId,
            cancellationToken);

        if (role != ProjectRole.Admin)
        {
            throw new ForbiddenException(
                "Only project admins can view all project tasks.");
        }
    }

    private static void Normalize(CreateTaskDto request)
    {
        request.Title = request.Title.Trim();
        request.Description =
            string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();
    }

    private static void Normalize(UpdateTaskDto request)
    {
        request.Title = request.Title.Trim();
        request.Description =
            string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();
    }
}