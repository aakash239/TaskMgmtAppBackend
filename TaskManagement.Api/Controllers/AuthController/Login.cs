using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.AuthController;

public partial class AuthController: ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken
    )
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        if (response is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Unathorized", detail: "Invalid Email and password");
        }
        return Ok(response);
    }
}