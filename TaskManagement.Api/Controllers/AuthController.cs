using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        if(response is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Unathorized", detail: "Invalid Email and password");
        }
        return Ok(response);
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("You are authenticated!!");
    }
}