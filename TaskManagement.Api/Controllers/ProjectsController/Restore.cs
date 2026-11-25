 
  using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.ProjectsController;

public partial class ProjectsController
{
 
 
 
 // POST /api/projects/5/restore : bring it back from the trash (admin only)
    [HttpPost("{id:int}/restore")]
    public async Task<ActionResult<ProjectDto>> Restore(int id, CancellationToken cancellationToken)
    => Ok(await _projectService.RestoreAsync(id, cancellationToken));


}