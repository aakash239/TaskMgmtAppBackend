using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IJwtTokenService
{
    JwtTokenDto GenerateAccessToken(JwtCreationDto user);
}

// Generate JWT Token
// $bytes = New-Object byte[] 64; $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create(); $rng.GetBytes($bytes); [Convert]::ToBase64String($bytes)