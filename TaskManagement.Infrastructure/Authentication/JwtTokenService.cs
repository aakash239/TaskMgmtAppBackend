using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Configuration;

namespace TaskManagement.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _options;

    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _options = options.Value;

        if(string.IsNullOrWhiteSpace(_options.Issuer) || 
            string.IsNullOrWhiteSpace(_options.Audience) ||
            string.IsNullOrWhiteSpace(_options.Key) || 
            Encoding.UTF8.GetByteCount(_options.Key) < 32 || 
            _options.AccessTokenExpiryMinutes <= 0)
        {
            throw new InvalidOperationException("JWT configuration is missing or invalid.");
        }   
    }

    public JwtTokenDto GenerateAccessToken(JwtCreationDto user)
    {
        // create claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // create signing key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        // create signing credentials
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // calculate expiration
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_options.AccessTokenExpiryMinutes);

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
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(token);

        // return result
        return new JwtTokenDto(tokenString, expires);
    }
}
