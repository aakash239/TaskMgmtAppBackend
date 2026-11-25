

using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
namespace TaskManagement.Api.Controllers.TaskController;

public partial class TaskController{
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

   
}