using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Api.Controllers.AuthController;

public partial class AuthController: ControllerBase
{
    // POST /api/auth/signup
    [HttpPost("signup")]
    [AllowAnonymous]                                            // no token needed to sign up
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SignUp([FromBody] UserSignUpDto request, CancellationToken cancellationToken)
    {
        var result = await _authService.SignUpAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);                 // 201 + body
    }
}