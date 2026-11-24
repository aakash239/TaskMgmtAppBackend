using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.BoardsController;

public partial class BoardsController
{
    // POST: api/boards
    [HttpPost]
    public async Task<IActionResult> Create(int ProjectId, CancellationToken cancellationToken)
    {
        var boardId = await _boardService.GetOrCreateMyBoardAsync(ProjectId, UserId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new {boardId}, new {boardId});
    }
}