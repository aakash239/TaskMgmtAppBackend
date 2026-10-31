using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagement.Api.Controllers.AuthController;

public partial class AuthController: ControllerBase
{
    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        await _authService.LogoutAsync(refreshToken, cancellationToken);
        Response.Cookies.Delete(RefreshCookieName, GetRefreshCookieOptions());
        return NoContent();
    }
}
