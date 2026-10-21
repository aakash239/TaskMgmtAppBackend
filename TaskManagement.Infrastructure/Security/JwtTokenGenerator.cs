using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Models;
using TaskManagement.Core.Configuration;

namespace TaskManagement.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    // IOptions<T> gives us the bound "Jwt" section from appsettings.json.
    public JwtTokenGenerator(IOptions<JwtSettings> options) => _settings = options.Value;

    public AccessTokenDto GenerateAccessToken(UserDto user)
    {
        // Claims = facts about the user stored inside the token.
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),   // subject = user id
            new(JwtRegisteredClaimNames.Email, user.Email),             // user's email
            new(JwtRegisteredClaimNames.Name, user.Name),               // user's name
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // unique token id
        };

        // Turn the secret string into a cryptographic key.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));

        // Sign with HMAC-SHA256. 
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Always UTC so server time zones don't matter.
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiryMinutes);

        // Assemble the token: header + payload (claims, iss, aud, exp) + signature.
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,   // not valid before now
            expires: expiresAt,           // not valid after this
            signingCredentials: credentials);

        // Serialize 
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenDto { Token = tokenString, ExpiresAtUtc = expiresAt };
    }
}