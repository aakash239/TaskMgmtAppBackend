using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Models;

namespace TaskManagement.Api.Controllers;

[ApiController]                 // enables automatic 400 responses when DTO validation fails
[Route("api/[controller]")]     // -> /api/auth
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

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

    // GET /api/auth/me: temporary endpoint 
    [HttpGet("me")]
    [Authorize]                                                 // requires a valid Bearer token
    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = User.FindFirstValue("sub"),   // read claims back out of the validated token
            Email = User.FindFirstValue("email"),
            Name = User.FindFirstValue("name")
        });
    }
}