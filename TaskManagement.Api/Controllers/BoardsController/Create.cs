using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.BoardsController;

public partial class BoardsController
{
    // POST: api/boards
    [HttpPost]
    public async Task<IActionResult> Create(CreateBoardDto dto, CancellationToken cancellationToken)
    {
        var templateId = await _boardService.CreateBoardAsync(UserId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new {templateId}, new {templateId});
    }
}