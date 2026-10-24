namespace TaskManagement.Application.Interfaces;

public interface IRefreshTokenGenerator
{
    abstract static string GenerateRefreshToken();
    abstract static string HashRefreshToken(string refreshToken);
}