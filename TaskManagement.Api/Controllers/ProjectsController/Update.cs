 using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.ProjectsController;

public partial class ProjectsController
{
 
 
 // PUT /api/projects/5 : update (admin only)
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectDto>> Update(int id, [FromBody] ProjectRequestDto request, CancellationToken cancellationToken)
    => Ok(await _projectService.UpdateAsync(id, request, cancellationToken));
}