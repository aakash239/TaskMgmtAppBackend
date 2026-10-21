namespace TaskManagement.Application.Models;

// Output of the token generator: the token string plus its expiry time.
public class AccessTokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}