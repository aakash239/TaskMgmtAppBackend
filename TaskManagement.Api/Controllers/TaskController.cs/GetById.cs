 
   using Microsoft.AspNetCore.Mvc;
   using TaskManagement.Application.DTOs;
   namespace TaskManagement.Api.Controllers.TaskController;

public partial class TaskController{
 
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
}