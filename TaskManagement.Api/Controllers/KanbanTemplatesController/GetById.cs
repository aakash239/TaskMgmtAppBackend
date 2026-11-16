using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Api.Controllers.KanbanTemplatesController;

public partial class KanbanTemplatesController
{
    // GET: api/boards/5
    [HttpGet("{TemplateId:int}")]
    public async Task<IActionResult> GetById(int templateId, CancellationToken cancellationToken)
    {
        var template = await _templateService.GetByIdAsync(templateId, UserId, cancellationToken);
        if(template == null)
            return NotFound();
        return Ok(template);
    }
}