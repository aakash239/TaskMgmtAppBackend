using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using System.Security.Claims;

namespace TaskManagement.Api.Controllers.AuthController;

[ApiController]
[Route("api/[controller]")]     // -> /api/auth
public partial class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    
    


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
    [HttpGet("test")]
    [Authorize]
    public IActionResult Test()
    {
        return Ok("You are authenticated!!");
    }
}