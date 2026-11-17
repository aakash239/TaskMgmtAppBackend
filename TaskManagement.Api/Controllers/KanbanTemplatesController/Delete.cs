using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Api.Controllers.KanbanTemplatesController;

public partial class KanbanTemplatesController
{
    // DELETE: api/boards/5
    [HttpDelete("{templateId:int}")]
    public async Task<IActionResult> Delete(int templateId, CancellationToken cancellationToken)
    {
        var deleted = await _templateService.DeleteAsync(templateId, UserId, cancellationToken);
        if(!deleted)
            return NotFound();
        return NoContent();
    }
}