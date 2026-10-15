using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;

        if(string.IsNullOrWhiteSpace(_options.Issuer) || 
            string.IsNullOrWhiteSpace(_options.Audience) ||
            string.IsNullOrWhiteSpace(_options.Key) || 
            Encoding.UTF8.GetByteCount(_options.Key) < 32 || 
            _options.ExpirationMinutes <= 0)
        {
            throw new InvalidOperationException("JWT configuration is missing or invalid.");
        }   
    }

    public JwtTokenDto GenerateToken(int userId, string email)
    {
        // create claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),  
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // create signing key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        // create signing credentials
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // calculate expiration
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_options.ExpirationMinutes);

        // create JWT
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials
        );

        //serialize JWT
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // return result
        return new JwtTokenDto(tokenString, expires);
    }
}