using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Models;
using WelfareTracker.Core.Options;

namespace WelfareTracker.Infrastructure.Service;
public class AuthService(IOptions<JwtOptions> options) : IAuthService
{
    private readonly JwtOptions _options = options.Value;
    public string GenerateJwtToken(User user)
    {
        var claims = new Claim[]
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim("Id", user.UserId.ToString()),
            new Claim(ClaimTypes.Role, user.RoleName ?? string.Empty),
            new Claim("ConstituencyName", user.ConstituencyName ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_options.ExpiryMinutes),
            signingCredentials: creds
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GeneratePasswordHash(string password, string salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 100000, HashAlgorithmName.SHA256);
        var hashBytes = pbkdf2.GetBytes(64);
        var hash = Convert.ToBase64String(hashBytes);

        return hash;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[32];
        rng.GetBytes(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);

        return salt;
    }

    public bool VerifyPassword(string userPassword, string salt, string passwordHash)
    {
        var hashOfInput = GeneratePasswordHash(userPassword, salt);
        return hashOfInput.Equals(passwordHash);
    }
}
