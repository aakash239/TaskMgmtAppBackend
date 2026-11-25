   
   using Microsoft.AspNetCore.Mvc;
   using TaskManagement.Application.DTOs;
   namespace TaskManagement.Api.Controllers.TaskController;

public partial class TaskController{
   
   
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
}