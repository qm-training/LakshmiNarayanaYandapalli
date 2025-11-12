namespace WelfareWorkTrackerAuth.Infrastructure.Service;
public class AuthService(IOptions<JwtOptions> options) : IAuthService
{
    private readonly JwtOptions _options = options.Value;
    public string GenerateJwtToken(User user)
    {
        var roleName = Enum.GetName(typeof(Roles), (Roles)user.RoleId) ?? string.Empty;
        var claims = new Claim[]
        {
        new Claim("Email", user.Email ?? string.Empty),
        new Claim("Id", user.UserId.ToString()),
        new Claim(ClaimTypes.Role, roleName ?? string.Empty),
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