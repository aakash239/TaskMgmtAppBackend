using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.AuthController;

public partial class AuthController : ControllerBase
{
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        var result = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);
        if (result == null)
        {
            // Do not clear the cookie: a stale request may finish after a successful rotation.
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Unauthorized",
                detail: "The refresh token is invalid or has expired.");
        }

        SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAtUtc);
        return Ok(result.Response);
    }
}
