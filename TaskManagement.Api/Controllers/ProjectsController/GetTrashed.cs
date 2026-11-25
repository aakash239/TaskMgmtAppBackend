 
 using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.ProjectsController;

public partial class ProjectsController
{
 
 
 
 // GET /api/projects/trash : trashed projects I administer, with days remaining

    [HttpGet("trash")]
    public async Task<ActionResult<List<TrashedProjectDto>>> GetTrash(CancellationToken cancellationToken)
    => Ok(await _projectService.GetMyTrashAsync(cancellationToken));

}