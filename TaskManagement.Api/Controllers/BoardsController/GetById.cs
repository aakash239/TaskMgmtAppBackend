using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Api.Controllers.BoardsController;

public partial class BoardsController
{
    // GET: api/boards/5
    [HttpGet("{boardId:int}")]
    public async Task<IActionResult> GetById(int boardId, CancellationToken cancellationToken)
    {
        var board = await _boardService.GetByIdAsync(boardId, UserId, cancellationToken);
        if(board == null)
            return NotFound();
        return Ok(board);
    }
}