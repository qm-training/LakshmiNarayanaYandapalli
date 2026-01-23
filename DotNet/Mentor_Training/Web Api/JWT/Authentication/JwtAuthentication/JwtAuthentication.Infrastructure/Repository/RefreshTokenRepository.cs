namespace JwtAuthentication.Infrastructure.Repository;
public class RefreshTokenRepository(JwtContext context) : IRefreshTokenRepository
{
    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        var refreshToken = await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        if (refreshToken == null)
        {
            return null;
        }
        return refreshToken;
    }

    public async Task<string> UpdateRefreshTokenAsync(RefreshToken refreshToken)
    {
        var existingToken = await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == refreshToken.UserId);
        existingToken!.Token = refreshToken.Token;
        existingToken.ExpireDate = refreshToken.ExpireDate;
        await context.SaveChangesAsync();
        return "Refresh token updated successfully";

    }
}
