 
 using Microsoft.AspNetCore.Mvc;
   using TaskManagement.Application.DTOs;
   namespace TaskManagement.Api.Controllers.TaskController;

public partial class TaskController{
 
 
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
}