using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers;

[ApiController] 
[Authorize] 
[Route("api/[controller]")] 
public class ProjectsController : ControllerBase
{
private readonly IProjectService _projectService;

public ProjectsController(IProjectService projectService) => _projectService = projectService;

// GET /api/projects : my active projects, each with my role in it
[HttpGet]
public async Task<ActionResult<List<ProjectDto>>> GetMine(CancellationToken cancellationToken)
=> Ok(await _projectService.GetMyProjectsAsync(cancellationToken));

// GET /api/projects/trash : trashed projects I administer, with days remaining

[HttpGet("trash")]
public async Task<ActionResult<List<TrashedProjectDto>>> GetTrash(CancellationToken cancellationToken)
=> Ok(await _projectService.GetMyTrashAsync(cancellationToken));

// GET /api/projects/5 : one project (members only)
[HttpGet("{id:int}")]
public async Task<ActionResult<ProjectDto>> GetById(int id, CancellationToken cancellationToken)
=> Ok(await _projectService.GetByIdAsync(id, cancellationToken));

// POST /api/projects : create. The caller becomes Admin of the new project.
[HttpPost]
public async Task<ActionResult<ProjectDto>> Create([FromBody] ProjectRequestDto request, CancellationToken cancellationToken)
{
var project = await _projectService.CreateAsync(request, cancellationToken);


return CreatedAtAction(nameof(GetById), new { id = project.ProjectId }, project);
}

// PUT /api/projects/5 : update (admin only)
[HttpPut("{id:int}")]
public async Task<ActionResult<ProjectDto>> Update(int id, [FromBody] ProjectRequestDto request, CancellationToken cancellationToken)
=> Ok(await _projectService.UpdateAsync(id, request, cancellationToken));

// DELETE /api/projects/5 : move to trash (admin only)
[HttpDelete("{id:int}")]
public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
{
await _projectService.DeleteAsync(id, cancellationToken);
return NoContent();
}

// POST /api/projects/5/restore : bring it back from the trash (admin only)
[HttpPost("{id:int}/restore")]
public async Task<ActionResult<ProjectDto>> Restore(int id, CancellationToken cancellationToken)
=> Ok(await _projectService.RestoreAsync(id, cancellationToken));
}