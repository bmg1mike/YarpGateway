using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sterling.Gateway.Domain;

namespace Sterling.Gateway.Application;

public static class Utilities
{
    public static string GenerateToken(GatewayApplication user, IConfiguration _config)
    {
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id),
            new Claim(ClaimTypes.Email,user.Email!),
            new Claim("role", "admin"),
            new Claim("Permission",user.PermissionRole)
        };



        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtConfig:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenOptions = new JwtSecurityToken
        (
            issuer: _config["JwtConfig:Issuer"], 
            audience: _config["JwtConfig:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt16(_config["JwtConfig:ExpirationTime"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    public static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
