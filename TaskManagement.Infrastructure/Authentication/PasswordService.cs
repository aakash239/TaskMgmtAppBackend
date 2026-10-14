using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Authentication;

public class PasswordSerice : IPasswordService
{
    private readonly PasswordHasher<object> _passwordHasher = new();
    private static readonly object UserContext = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(UserContext, password);
    }

    public bool VerifyPassword(string HashedPassword, string ProvidedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(UserContext, HashedPassword, ProvidedPassword);
        return result != PasswordVerificationResult.Failed;
    }
}