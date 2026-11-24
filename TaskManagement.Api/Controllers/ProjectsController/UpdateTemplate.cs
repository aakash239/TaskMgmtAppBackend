using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.ProjectsController;

public partial class ProjectsController
{
    //PUT /api/projects/5/template
    [HttpPut("{id:int}/template")]
    public async Task<ActionResult<ProjectDto>> UpdateTemplate(int id, [FromBody] UpdateProjectTemplateDto request, CancellationToken cancellationToken)
    {
        return Ok(await _projectService.UpdateTemplateAsync(id, request, cancellationToken));
    }
}