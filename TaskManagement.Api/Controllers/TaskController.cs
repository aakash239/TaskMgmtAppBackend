using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/projects/{projectId:int}/tasks")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    private readonly ITaskService _taskService = taskService;

    // A member sees only their own tasks.
    // An admin sees their own tasks here too.
    [HttpGet]
    public async Task<ActionResult<List<TaskDto>>> GetMyTasks(
        int projectId,
        CancellationToken cancellationToken)
    {
        var tasks = await _taskService.GetMyTasksAsync(
            projectId,
            cancellationToken);

        return Ok(tasks);
    }

    // Admin-only: all tasks belonging to every board in this project.
    [HttpGet("all")]
    public async Task<ActionResult<List<TaskDto>>> GetAllProjectTasks(
        int projectId,
        CancellationToken cancellationToken)
    {
        var tasks = await _taskService.GetAllProjectTasksAsync(
            projectId,
            cancellationToken);

        return Ok(tasks);
    }

    [HttpGet("{taskId:int}")]
    public async Task<ActionResult<TaskDto>> GetById(
        int projectId,
        int taskId,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.GetByIdAsync(
            projectId,
            taskId,
            cancellationToken);

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(
        int projectId,
        [FromBody] CreateTaskDto request,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.CreateAsync(
            projectId,
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { projectId, taskId = task.TaskId },
            task);
    }

    [HttpPut("{taskId:int}")]
    public async Task<ActionResult<TaskDto>> Update(
        int projectId,
        int taskId,
        [FromBody] UpdateTaskDto request,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.UpdateAsync(
            projectId,
            taskId,
            request,
            cancellationToken);

        return Ok(task);
    }

    [HttpDelete("{taskId:int}")]
    public async Task<IActionResult> Delete(
        int projectId,
        int taskId,
        CancellationToken cancellationToken)
    {
        await _taskService.DeleteAsync(
            projectId,
            taskId,
            cancellationToken);

        return NoContent();
    }
}