  using Microsoft.AspNetCore.Mvc;
   using TaskManagement.Application.DTOs;
   namespace TaskManagement.Api.Controllers.TaskController;

public partial class TaskController{
 
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
}