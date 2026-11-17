using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Api.Controllers.BoardsController;

public partial class BoardsController
{
    // DELETE: api/boards/5
    [HttpDelete("{boardId:int}")]
    public async Task<IActionResult> Delete(int boardId, CancellationToken cancellationToken)
    {
        var deleted = await _boardService.DeleteBoardByIdAsync(boardId, UserId, cancellationToken);
        if(deleted)
            return NotFound();
        return NoContent();
    }
}