using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.KanbanTemplatesController;

public partial class KanbanTemplatesController
{
        // POST: api/boards
    [HttpPost]
    public async Task<IActionResult> Create(CreateKanbanTemplateDto dto, CancellationToken cancellationToken)
    {
        var templateId = await _templateService.CreateTemplateAsync(UserId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new {templateId}, new {templateId});
    }
}