using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TaskManagement.Api.Controllers.AuthController;

public partial class AuthController: ControllerBase
{
    [HttpPost("logout-all")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
    {
        if (!int.TryParse(User.FindFirstValue("sub"), out var userId) || userId <= 0)
        {
            return Unauthorized();
        }

        await _authService.LogoutAllAsync(userId, cancellationToken);
        ClearRefreshTokenCookie();
        return NoContent();
    }
}
