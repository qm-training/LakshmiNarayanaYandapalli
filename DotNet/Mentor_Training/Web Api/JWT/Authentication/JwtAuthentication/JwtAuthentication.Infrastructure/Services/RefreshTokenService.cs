namespace JwtAuthentication.Infrastructure.Services;
public class RefreshTokenService(IRefreshTokenRepository repository, IUserRepository userRepository) : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _repository = repository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<string> AddRefreshTokenAsync(string userName)
    {
        var token = GenerateRefreshToken();
        var user = await _userRepository.GetUserByNameAsync(userName);
        if (user == null)
        {
            return "User not found";
        }
        RefreshToken refreshToken = new()
        {
            Token = token,
            ExpireDate = DateTime.UtcNow.AddDays(7),
            UserId = user.UserId
        };

        await _repository.AddRefreshTokenAsync(refreshToken);
        return token;
    }

    public async Task<string> RenewRefreshTokenAsync(string userName, string refreshToken)
    {
        var newToken = GenerateRefreshToken();
        var user = await _userRepository.GetUserByNameAsync(userName);
        if (user == null)
        {
            return "User not found";
        }
        var token = await _repository.GetRefreshTokenAsync(refreshToken);
        if (token == null || token.UserId != user.UserId || token.ExpireDate < DateTime.UtcNow)
        {
            return "Invalid refresh token";
        }
        token.Token = newToken;
        token.ExpireDate = DateTime.UtcNow.AddDays(7);
        await _repository.UpdateRefreshTokenAsync(token);
        return newToken;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string userName, string refreshToken)
    {
        var user = await _userRepository.GetUserByNameAsync(userName);
        if (user == null)
        {
            return false;
        }
        var token = await _repository.GetRefreshTokenAsync(refreshToken);
        if (token == null || token.UserId != user.UserId || token.ExpireDate < DateTime.UtcNow)
        {
            return false;
        }
        return true;
    }

    public static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
