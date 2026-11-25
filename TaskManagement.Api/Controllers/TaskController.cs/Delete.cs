

    using Microsoft.AspNetCore.Mvc;
   
   namespace TaskManagement.Api.Controllers.TaskController;

public partial class TaskController{
 
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