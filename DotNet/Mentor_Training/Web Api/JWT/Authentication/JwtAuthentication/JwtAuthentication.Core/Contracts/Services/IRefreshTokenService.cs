namespace JwtAuthentication.Core.Contracts.Services;
public interface IRefreshTokenService
{
    Task<string> AddRefreshTokenAsync(string userName);
    Task<bool> ValidateRefreshTokenAsync(string userName, string refreshToken);
    Task<string> RenewRefreshTokenAsync(string userName, string refreshToken);
}
