using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Api.Controllers.BoardsController;

public partial class BoardsController
{
    // GET: api/boards
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var boards = await _boardService.GetAllByUserIdAsync(UserId, cancellationToken);
        return Ok(boards);
    }
}