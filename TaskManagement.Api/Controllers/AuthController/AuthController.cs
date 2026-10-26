using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace TaskManagement.Api.Controllers.AuthController;

[ApiController]
[Route("api/[controller]")]     // -> /api/auth
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public partial class AuthController : ControllerBase, IActionFilter
{
    private const string RefreshCookieName = "refreshToken";
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _environment;
    private readonly string[] _allowedOrigins;

    public AuthController(IAuthService authService, IWebHostEnvironment environment, IConfiguration configuration)
    {
        _authService = authService;
        _environment = environment;
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (allowedOrigins == null)
        {
            if (environment.IsDevelopment())
            {
                allowedOrigins = ["http://localhost:4200"];
            }
            else
            {
                allowedOrigins = [];
            }
        }

        _allowedOrigins = allowedOrigins;
    }

    [NonAction]
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (HttpMethods.IsPost(Request.Method))
        {
            var csrfHeader = Request.Headers["X-CSRF"];
            bool validHeader = csrfHeader.Count == 1 && csrfHeader[0] == "1";
            bool validOrigin = true;

            // REST clients may omit Origin. Browsers must use our Angular origin.
            if (Request.Headers.TryGetValue("Origin", out var origin))
            {
                validOrigin = origin.Count == 1 && _allowedOrigins.Contains(origin[0]);
            }

            if (!validHeader || !validOrigin)
            {
                context.Result = Problem(statusCode: StatusCodes.Status403Forbidden,
                    title: "Forbidden", detail: "Send X-CSRF: 1 and use an allowed browser origin.");
                return;
            }
        }

    }

    [NonAction]
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    private void SetRefreshTokenCookie(string token, DateTime expiresAt)
    {
        var options = GetRefreshCookieOptions();
        var utcExpiry = DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc);
        options.Expires = new DateTimeOffset(utcExpiry);
        Response.Cookies.Append(RefreshCookieName, token, options);
    }

    private void ClearRefreshTokenCookie()
    {
        Response.Cookies.Delete(RefreshCookieName, GetRefreshCookieOptions());
    }

    private CookieOptions GetRefreshCookieOptions()
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth",
            IsEssential = true
        };

        bool isLocalhost = string.Equals(Request.Host.Host, "localhost", StringComparison.OrdinalIgnoreCase);
        if (_environment.IsDevelopment() && isLocalhost && !Request.IsHttps)
        {
            options.Secure = false;
        }

        return options;
    }

    // GET /api/auth/me
    [HttpGet("me")]
    [Authorize]
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
