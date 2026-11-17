namespace TaskManagement.Application.Interfaces;

public interface IRefreshTokenGenerator
{
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}