using System.Security.Cryptography;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Security;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public static string GenerateRefreshToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(64); // 64 secure bytes
        return Convert.ToBase64String(randomBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_'); // ensuring our generated code is url safe. 
    }

    public static string HashRefreshToken(string refreshToken)
    {
        byte[] tokenBytes = System.Text.Encoding.UTF8.GetBytes(refreshToken);
        byte[] hashBytes = SHA256.HashData(tokenBytes); // 256 bit hash regardless of token's original length

        return Convert.ToHexString(hashBytes);
    }
}