 
 
 using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.ProjectsController;

public partial class ProjectsController
{
 
 // GET /api/projects/5 : one project (members only)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id, CancellationToken cancellationToken)
    => Ok(await _projectService.GetByIdAsync(id, cancellationToken));

}