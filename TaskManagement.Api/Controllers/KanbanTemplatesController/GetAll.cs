using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Api.Controllers.KanbanTemplatesController;

public partial class KanbanTemplatesController
{
    // GET: api/kanban-templates
    [HttpGet]
    public async Task<IActionResult> GetAll(int templateid, CancellationToken cancellationToken = default)
    {
        var templates = await _templateService.GetAllAsync(UserId, cancellationToken);
        return Ok(templates);
    }
}