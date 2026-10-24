using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.AuthController;

public partial class AuthController : ControllerBase
{
    [HttpPost("refresh")]
    [Authorize]
    public async Task<ActionResult<TokenRefreshResponseDto>> Refresh(
        [FromBody] TokenRefereshDto request,
        CancellationToken cancellationToken
    )
    {   
        var response = await _authService.RefreshTokenAsync(request, cancellationToken);
    }     
} 