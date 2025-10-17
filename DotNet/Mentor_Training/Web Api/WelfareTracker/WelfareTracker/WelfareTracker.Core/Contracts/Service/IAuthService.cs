using WelfareTracker.Core.Models;

namespace WelfareTracker.Core.Contracts.Service;
public interface IAuthService
{
    string GeneratePasswordHash(string password, string salt);
    string GenerateSalt();
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    bool VerifyPassword(string userPassword, string salt, string passwordHash);

}
