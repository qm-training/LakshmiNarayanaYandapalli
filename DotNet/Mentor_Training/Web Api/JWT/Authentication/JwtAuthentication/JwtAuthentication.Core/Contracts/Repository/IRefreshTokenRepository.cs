namespace JwtAuthentication.Core.Contracts.Repository;
public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task<string> UpdateRefreshTokenAsync(RefreshToken refreshToken);
}
