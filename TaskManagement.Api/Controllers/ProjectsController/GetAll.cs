
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.ProjectsController;

public partial class ProjectsController
{

// GET /api/projects : my active projects, each with my role in it
    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetMine(CancellationToken cancellationToken)
=> Ok(await _projectService.GetMyProjectsAsync(cancellationToken));
}
