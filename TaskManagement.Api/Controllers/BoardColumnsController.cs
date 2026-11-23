using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/boards/{boardId:int}/columns")]
[Authorize]
public class BoardColumnsController(
    IBoardService boardService) : ControllerBase
{
    private readonly IBoardService _boardService = boardService;

    private int UserId => int.Parse(User.FindFirstValue("sub")!);

    // GET: api/boards/1/columns
    [HttpGet]
    public async Task<IActionResult> GetColumns(
        int boardId,
        CancellationToken cancellationToken)
    {
        var columns = await _boardService.GetBoardColumnsAsync(
            boardId, UserId, cancellationToken);

        return Ok(columns);
    }

    // POST: api/boards/1/columns
    [HttpPost]
    public async Task<IActionResult> AddColumn(
        int boardId,
        CreateBoardColumnDto dto,
        CancellationToken cancellationToken)
    {
        var columnId = await _boardService.AddBoardColumnAsync(
            boardId, UserId, dto, cancellationToken);

        return CreatedAtAction(
            nameof(GetColumns),
            new { boardId },
            new { columnId });
    }

    // PUT: api/boards/1/columns/2
    [HttpPut("{columnId:int}")]
    public async Task<IActionResult> UpdateColumn(
        int boardId,
        int columnId,
        UpdateBoardColumnDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await _boardService.UpdateBoardColumnAsync(
            boardId, columnId, UserId, dto, cancellationToken);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    // DELETE: api/boards/1/columns/2
    [HttpDelete("{columnId:int}")]
    public async Task<IActionResult> DeleteColumn(
        int boardId,
        int columnId,
        CancellationToken cancellationToken)
    {
        var deleted = await _boardService.DeleteBoardColumnAsync(
            boardId, columnId, UserId, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    // PATCH: api/boards/1/columns/reorder
    [HttpPatch("reorder")]
    public async Task<IActionResult> ReorderColumns(
        int boardId,
        ReorderColumnsDto dto,
        CancellationToken cancellationToken)
    {
        var reordered = await _boardService.ReorderBoardColumnsAsync  (
            boardId, UserId, dto, cancellationToken);

        if (!reordered)
            return NotFound();

        return NoContent();
    }
}