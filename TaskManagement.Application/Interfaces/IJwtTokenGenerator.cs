using TaskManagement.Application.Models;


namespace TaskManagement.Application.Interfaces;

// Contract for creating JWT access tokens. Implemented in Infrastructure.
public interface IJwtTokenGenerator
{
    AccessTokenDto GenerateAccessToken(UserDto user);
}