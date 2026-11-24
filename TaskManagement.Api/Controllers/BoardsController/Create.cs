using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Api.Controllers.BoardsController;

public partial class BoardsController
{
    // POST: api/boards
    [HttpGet("~/api/projects/{ProjectId:int}/boards/me")]
    public async Task<IActionResult> Create(int ProjectId, CancellationToken cancellationToken)
    {
        var board = await _boardService.GetOrCreateMyBoardAsync(ProjectId, UserId, cancellationToken);
        return Ok(board);
    }
}