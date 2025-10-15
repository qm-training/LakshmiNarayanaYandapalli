namespace JwtAuthentication.Infrastructure.Services;
public class JwtService(IConfiguration configuration)
{
    private readonly string _secretKey = configuration["JWT:Key"]!;
    private readonly string _issuer = configuration["JWT:Issuer"]!;
    private readonly string _audience = configuration["JWT:Audience"]!;

    public string GenerateToken(UserVm user)
    {
        var claims = new Claim[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: creds
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
