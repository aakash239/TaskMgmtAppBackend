
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.ProjectsController;

public partial class ProjectsController
{

    // GET /api/projects/{projectId/all-tasks} : All tasks in a project for the admin to view
    [HttpGet("/{projectId:int}/all-tasks")]
    public async Task<ActionResult<List<ProjectDto>>> GetAllTasks(int projectId, CancellationToken cancellationToken) => Ok(await _projectService.GetAllTasksAsync(projectId, cancellationToken));
}
