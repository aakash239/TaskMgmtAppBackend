using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Api.Controllers.BoardsController;

[ApiController]
[Route("api/boards")]
[Authorize]
public partial class BoardsController(IBoardService boardService) : ControllerBase
{
    private readonly IBoardService _boardService = boardService;
    private int UserId => int.Parse(User.FindFirstValue("sub")!);
}