
 using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.ProjectsController;

public partial class ProjectsController
{

// POST /api/projects : create. The caller becomes Admin of the new project.
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] ProjectRequestDto request, CancellationToken cancellationToken)
    {
        var project = await _projectService.CreateProjectAsync(request, cancellationToken);


        return CreatedAtAction(nameof(GetById), new { id = project.ProjectId }, project);
    }

}