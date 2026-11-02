using System.Security.Claims;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Api;

// Reads the logged-in user's ID from the validated JWT.
public class CurrentUser : ICurrentUserService
{
private readonly IHttpContextAccessor _httpContextAccessor; // gives access to the current request

public CurrentUser(IHttpContextAccessor httpContextAccessor) =>
_httpContextAccessor = httpContextAccessor;

public int UserId
{
get
{

var value = _httpContextAccessor.HttpContext?.User.FindFirstValue("sub");


if (!int.TryParse(value, out var userId))
throw new InvalidOperationException("Authenticated user id could not be read from the token.");

return userId;
}
}
}
